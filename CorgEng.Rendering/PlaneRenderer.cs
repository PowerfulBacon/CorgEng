﻿using CorgEng.Core;
using CorgEng.Core.Dependencies;
using CorgEng.Core.Rendering;
using CorgEng.GenericInterfaces.Core;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Rendering;
using CorgEng.GenericInterfaces.Rendering.Renderers;
using CorgEng.GenericInterfaces.Rendering.Shaders;
using CorgEng.GenericInterfaces.Rendering.SharedRenderAttributes;
using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.GenericInterfaces.UtilityTypes.Batches;
using System;
using static OpenGL.Gl;

namespace CorgEng.Rendering
{
	public abstract partial class PlaneRenderer : InstancedRendererDependencyHolder, IDisposable
	{

		private static float[] quadVertices = {
			1, 1, 0,
			1, -1, 0,
			-1, 1, 0,
			-1, 1, 0,
			1, -1, 0,
			-1, -1, 0
		};

		private static bool initialized = false;

		//The vertex array and buffer objects
		private static uint vertexArray;
		private static uint vertexBuffer;

		private static IShaderSet shaderSet;

		private static int textureUniformLocation;

		//Create a program for rendering
		private static uint planeDrawingProgramUint;

		/// <summary>
		/// The uint of the frame buffer
		/// </summary>
		public uint FrameBufferUint { get; private set; }

		/// <summary>
		/// The uint of the render buffer
		/// </summary>
		public uint RenderBufferUint { get; private set; }

		/// <summary>
		/// The uint of our render texture
		/// </summary>
		public uint RenderTextureUint { get; private set; }

		public int Width { get; internal set; } = 1920;

		public int Height { get; internal set; } = 1080;

		private static IColour _currentBackColour;
		private static DepthModes _currentDepthMode = DepthModes.KEEP_DEPTH;
		private static RenderModes _currentBlendMode = RenderModes.DEFAULT;

		/// <summary>
		/// The render mode to use when drawing this render core to the framebuffer.
		/// </summary>
		public virtual RenderModes DrawMode { get; } = RenderModes.DEFAULT;

		/// <summary>
		/// The render mode to use when rendering elements on to the render core.
		/// </summary>
		public virtual RenderModes BlendMode { get; } = RenderModes.DEFAULT;

		/// <summary>
		/// Should we keep depth?
		/// Ignoring depth causes the render core to overlay on whatever it is being drawn on.
		/// </summary>
		public virtual DepthModes DepthMode { get; } = DepthModes.KEEP_DEPTH;

		/// <summary>
		/// The path of the shaders to use.
		/// Only accessed during initialisation.
		/// </summary>
		public virtual string ShaderPath => "CoreShader";

		public virtual IColour BackColour { get; } = ColourFactory.GetColour(0, 0, 0, 0);

		public unsafe virtual void Initialize()
		{
			if (!CorgEngMain.IsRendering)
				return;
			//Generate a render buff
			RenderBufferUint = glGenRenderbuffer();
			glBindRenderbuffer(RenderBufferUint);
			glRenderbufferStorage(GL_RENDERBUFFER, GL_DEPTH_COMPONENT, Width, Height);
			//Generate a frame buffer
			FrameBufferUint = glGenFramebuffer();
			glBindFramebuffer(GL_FRAMEBUFFER, FrameBufferUint);
			//Create a render texture
			glActiveTexture(GL_TEXTURE0);
			RenderTextureUint = glGenTexture();
			//Bind the created texture so we can modify it
			glBindTexture(GL_TEXTURE_2D, RenderTextureUint);
			//Load the texture scale
			glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, Width, Height, 0, GL_RGBA, GL_UNSIGNED_BYTE, NULL);
			//Set the texture parameters
			glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
			glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
			//Bind the render depth buffer to the framebuffer
			glFramebufferRenderbuffer(GL_FRAMEBUFFER, GL_DEPTH_ATTACHMENT, GL_RENDERBUFFER, RenderBufferUint);
			//Bind the framebuffer to the texture
			glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D, RenderTextureUint, 0);
			//Check for issues
			if (glCheckFramebufferStatus(GL_FRAMEBUFFER) != GL_FRAMEBUFFER_COMPLETE)
			{
				//TODO: Introduce a rendering mode that bypasses framebuffers and just draws directly to the screen.
				//Slightly broken is better than nothing.
				Logger.WriteLine("WARNING: FRAMEBUFFER ERROR. Your GPU may not support this application!", LogType.ERROR);
			}
			//Log creation
			Logger?.WriteLine($"Created RenderCore, rendering to framebuffer {FrameBufferUint} outputting texture to {RenderTextureUint}", LogType.DEBUG);
			// Prepare for rendering if we need
			SetupRendering();
		}

		public unsafe static void SetupRendering()
		{
			//Create stuff we need for screen rendering (static)
			if (!initialized)
			{
				Logger?.WriteLine("Initialized RenderCore static requirements.", LogType.LOG);
				//Mark initialized to be true.
				initialized = true;
				//create and link a program
				planeDrawingProgramUint = glCreateProgram();
				//Start using the program: All operations will affect this program
				glUseProgram(planeDrawingProgramUint);
				//Generate a vertex array and bind it
				vertexArray = glGenVertexArray();
				glBindVertexArray(vertexArray);
				//Create and bind the vertex buffer
				vertexBuffer = glGenBuffer();
				glBindBuffer(GL_ARRAY_BUFFER, vertexBuffer);

				//Put data into the buffer
				fixed (float* vertexPointer = &quadVertices[0])
				{
					glBufferData(GL_ARRAY_BUFFER, sizeof(float) * quadVertices.Length, vertexPointer, GL_STATIC_DRAW);
				}
				//Create the shaders
				shaderSet = ShaderFactory.CreateShaderSet("CoreShader");
				//Get the uniform location of the shaders
				shaderSet.AttachShaders(planeDrawingProgramUint);
				glLinkProgram(planeDrawingProgramUint);
				//Fetch uniform locations
				textureUniformLocation = glGetUniformLocation(planeDrawingProgramUint, "renderTexture");
			}
		}

		/// <summary>
		/// Do rendering
		/// </summary>
		public void DoRender(ICamera camera)
		{
			RenderModes prev = SwitchBlendMode(BlendMode);
			DepthModes prevDepth = SwitchDepthMode(DepthMode);
			IColour prevColour = SwitchBackColour(BackColour);
			PreRender();
			Render(camera);
			SwitchBlendMode(prev);
			SwitchDepthMode(prevDepth);
			SwitchBackColour(prevColour);
		}

		public void PreRender()
		{
			//Bind our framebuffer to render to
			glBindFramebuffer(GL_FRAMEBUFFER, FrameBufferUint);
			//Clear the backbuffer
			glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
			//Set the viewport
			glViewport(0, 0, Width, Height);
		}

		/// <summary>
		/// Render whatever we need to render to the plane.
		/// </summary>
		/// <param name="camera"></param>
		public abstract void Render(ICamera camera);

		protected static RenderModes SwitchBlendMode(RenderModes newMode)
		{
			if (newMode == _currentBlendMode)
				return newMode;
			RenderModes prevMode = _currentBlendMode;
			_currentBlendMode = newMode;
			switch (_currentBlendMode)
			{
				case RenderModes.DEFAULT:
					glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
					break;
				case RenderModes.MULTIPLY:
					glBlendFunc(GL_DST_COLOR, GL_ZERO);
					break;
				case RenderModes.ADDITIVE:
					glBlendFunc(GL_ONE, GL_ONE);
					break;
			}
			return prevMode;
		}

		protected static DepthModes SwitchDepthMode(DepthModes newMode)
		{
			if (newMode == _currentDepthMode)
				return newMode;
			DepthModes prevMode = _currentDepthMode;
			_currentDepthMode = newMode;
			switch (_currentDepthMode)
			{
				case DepthModes.KEEP_DEPTH:
					glDepthFunc(GL_LEQUAL);
					break;
				case DepthModes.IGNORE_DEPTH:
					glDepthFunc(GL_ALWAYS);
					break;
			}
			return prevMode;
		}

		protected static IColour SwitchBackColour(IColour newColour)
		{
			if (newColour == null || newColour.Equals(_currentBackColour))
				return newColour;
			IColour prevCol = _currentBackColour;
			_currentBackColour = newColour;
			glClearColor(newColour.Red, newColour.Green, newColour.Blue, newColour.Alpha);
			return prevCol;
		}

		public unsafe void Resize(int width, int height)
		{
			//Set the new width
			Width = width;
			Height = height;
			//Log
			Logger?.WriteLine($"Render core resized to {Width}x{Height}", LogType.DEBUG);
			if (!CorgEngMain.IsRendering)
				return;
			//Update the tex image
			//Bind the created texture so we can modify it
			glBindTexture(GL_TEXTURE_2D, RenderTextureUint);
			//Load the texture scale
			glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, Width, Height, 0, GL_RGBA, GL_UNSIGNED_BYTE, NULL);
			//Set the texture parameters
			glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
			glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
		}

		public unsafe void DrawToBuffer(uint buffer, int drawX, int drawY, int bufferWidth, int bufferHeight)
		{
			//Reset the framebuffer (We want to draw to the screen, not a frame buffer)
			glBindFramebuffer(GL_FRAMEBUFFER, buffer);
			//Draw to full screen
			glViewport(drawX, drawY, bufferWidth, bufferHeight);

			//Setup blending
			RenderModes prevRender = SwitchBlendMode(DrawMode);
			DepthModes prevBlend = SwitchDepthMode(DepthMode);
			IColour prevColour = SwitchBackColour(BackColour);

			//Set the using program to our program uint
			glUseProgram(planeDrawingProgramUint);
			//Bind uniform variables
			glUniform1ui(textureUniformLocation, 0);
			//Bind the vertex buffer
			glEnableVertexAttribArray(0);
			glBindBuffer(GL_ARRAY_BUFFER, vertexBuffer);
			glVertexAttribPointer(
				0,                  //Attribute - Where the layout location is in the vertex shader
				3,                  //Size of the triangles (3 sides)
				GL_FLOAT,           //Type (Floats)
				false,              //Normalized (nope)
				0,                  //Stride (0)
				NULL                //Array buffer offset (null)
			);
			//Set the vertex attrib divisor
			glVertexAttribDivisor(0, 0);
			//Bind the texture
			glActiveTexture(GL_TEXTURE0);
			glBindTexture(GL_TEXTURE_2D, RenderTextureUint);
			//Draw
			glDrawArrays(GL_TRIANGLES, 0, 6);
			//Disable the vertex attrib array
			glDisableVertexAttribArray(0);

			SwitchBlendMode(prevRender);
			SwitchDepthMode(prevBlend);
			SwitchBackColour(prevColour);
		}

		/// <summary>
		/// Dispose the render core and cleanup any resources
		/// </summary>
		public void Dispose()
		{
			glDeleteRenderbuffer(RenderBufferUint);
			glDeleteFramebuffer(FrameBufferUint);
			glDeleteTexture(RenderTextureUint);
		}

	}
}
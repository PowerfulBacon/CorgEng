using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.Core;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Rendering.Shaders;
using System;
using static OpenGL.Gl;

namespace CorgEng.Core.Rendering
{
    public abstract class RenderCore : IRenderCore
    {

        [UsingDependency]
        private static ILogger Logger;

        //Fetch shader loading from some dependency somewhere
        [UsingDependency]
        private static IShaderFactory ShaderFactory;

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
        private static uint programUint;

        /// <summary>
        /// The uint of the frame buffer
        /// </summary>
        public uint FrameBufferUint { get; }

        /// <summary>
        /// The uint of the render buffer
        /// </summary>
        public uint RenderBufferUint { get; }

        /// <summary>
        /// The uint of our render texture
        /// </summary>
        public uint RenderTextureUint { get; }

        public int Width { get; internal set; } = 1920;

        public int Height { get; internal set; } = 1080;

        private static int CurrentBlendSource = GL_SRC_ALPHA;

        private static int CurrentBlendDestination = GL_ONE_MINUS_SRC_ALPHA;

        public virtual int BlendSource { get; } = GL_SRC_ALPHA;

        public virtual int BlendDestination { get; } = GL_ONE_MINUS_SRC_ALPHA;

        public unsafe RenderCore()
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
            //Bind the render buffer to the framebuffer
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
                programUint = glCreateProgram();
                //Start using the program: All operations will affect this program
                glUseProgram(programUint);
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
                shaderSet.AttachShaders(programUint);
                glLinkProgram(programUint);
                //Fetch uniform locations
                textureUniformLocation = glGetUniformLocation(programUint, "renderTexture");
            }
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
        /// Do rendering
        /// </summary>
        public void DoRender()
        {
            PreRender();
            PerformRender();
        }

        /// <summary>
        /// Called when the render core is initialized
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Perform rendering
        /// </summary>
        public abstract void PerformRender();

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
            if (CurrentBlendDestination != BlendDestination || CurrentBlendSource != BlendSource)
            {
                glBlendFunc(BlendSource, BlendDestination);
                CurrentBlendDestination = BlendDestination;
                CurrentBlendSource = BlendSource;
            }

            //Set the using program to our program uint
            glUseProgram(programUint);
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
        }
    }
}

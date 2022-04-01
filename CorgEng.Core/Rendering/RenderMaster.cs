using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.Rendering.Shaders;
using System;
using static OpenGL.Gl;

namespace CorgEng.Core.Rendering
{
    internal class RenderMaster
    {

        public int Width { get; set; } = 1920;

        public int Height { get; set; } = 1080;

        private static float[] quadVertices = {
            1, 1, 0,
            1, -1, 0,
            -1, 1, 0,
            -1, 1, 0,
            1, -1, 0,
            -1, -1, 0
        };

        //The vertex array and buffer objects
        uint vertexArray;
        uint vertexBuffer;

        private IShaderSet shaderSet;

        private int textureUniformLocation;

        //Fetch shader loading from some dependency somewhere
        [UsingDependency]
        private static IShaderFactory ShaderFactory;

        //Create a program for rendering
        private uint programUint;

        /// <summary>
        /// Initialize the internal render master
        /// </summary>
        public void Initialize()
        {
            //Setup the global GL flags
            SetGlobalGlFlags();
            //Create the screen renderer
            CreateScreenRenderer();
        }

        private void SetGlobalGlFlags()
        {
            //Enable backface culling
            glEnable(GL_CULL_FACE);
            glCullFace(GL_BACK);
            glFrontFace(GL_CW);
            //Enable depth test -> We use z to represent the layer an object is on
            glEnable(GL_DEPTH_TEST);
            //Use the lowest number fragment
            glDepthFunc(GL_LEQUAL);
            //Enable blending for transparent objects
            glEnable(GL_BLEND);
            glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
            //Set a background colour
            glClearColor(0, 0, 0, 1.0f);
        }

        private unsafe void CreateScreenRenderer()
        {
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
            shaderSet = ShaderFactory?.CreateShaderSet("CoreShader");
            //Get the uniform location of the shaders
            shaderSet.AttachShaders(programUint);
            glLinkProgram(programUint);
            //Fetch uniform locations
            textureUniformLocation = glGetUniformLocation(programUint, "renderTexture");
            //Detatch shaders
            //shaderSet.DetatchShaders(programUint);
        }

        /// <summary>
        /// Renders an image outputted by a render core to the screen.
        /// </summary>
        public unsafe void RenderImageToScreen(uint glImageUint)
        {
            //Reset the framebuffer (We want to draw to the screen, not a frame buffer)
            glBindFramebuffer(GL_FRAMEBUFFER, 0);
            //Clear the screen and reset it to the background colour
            glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
            //Render the image to the screen (create a basic textured quad and feed in the)
            //We want to render to the entire screen
            glViewport(0, 0, Width, Height);
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
            glBindTexture(GL_TEXTURE_2D, glImageUint);
            //Draw
            glDrawArrays(GL_TRIANGLES, 0, 6);
            //Disable the vertex attrib array
            glDisableVertexAttribArray(0);
        }

    }
}

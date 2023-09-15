using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.Rendering.Renderers;
using CorgEng.GenericInterfaces.Rendering.Shaders;
using System;
using static OpenGL.Gl;

namespace CorgEng.Core.Rendering
{
    internal class RenderMaster
    {

        public int Width { get; set; } = 1920;

        public int Height { get; set; } = 1080;

        /// <summary>
        /// Initialize the internal render master
        /// </summary>
        public void Initialize()
        {
            //Setup the global GL flags
            SetGlobalGlFlags();
        }

        private void SetGlobalGlFlags()
        {
            //Enable backface culling
            glEnable(GL_CULL_FACE);
            glCullFace(GL_BACK);
            glFrontFace(GL_CW);
            //Enable blending for transparent objects
            glEnable(GL_BLEND);
            glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
            //glBlendEquationSeparate(GL_FUNC_ADD, GL_FUNC_ADD);
            //Colour = (alpha*S) + (1-alpha) * d
            //glBlendFuncSeparate(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA, GL_ONE, GL_ZERO);
            //glBlendFunc(GL_DST_COLOR, GL_ZERO);
            //Colour = D*S + 0*D
            //Alpha = 1*S + 0*D
            //glBlendFuncSeparate(GL_DST_COLOR, GL_ZERO, GL_ONE, GL_ZERO);
            //Enable depth test -> We use z to represent the layer an object is on
            glEnable(GL_DEPTH_TEST);
            //Use the lowest number fragment
            glDepthFunc(GL_LEQUAL);
            //Set a background colour
            glClearColor(0, 0, 0, 0);

        }

        /// <summary>
        /// Renders an image outputted by a render core to the screen.
        /// </summary>
        public unsafe void RenderImageToScreen(IRenderer renderCore)
        {
            //Reset the framebuffer (We want to draw to the screen, not a frame buffer)
            glBindFramebuffer(GL_FRAMEBUFFER, 0);
            //Clear the screen and reset it to the background colour
            glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
            //Draw the render core to the screen
            renderCore.DrawToBuffer(0, 0, 0, Width, Height);
        }

        /// <summary>
        /// Set the window width and height
        /// </summary>
        internal void SetWindowRenderSize(int width, int height)
        {
            Width = width;
            Height = height;
        }

    }
}

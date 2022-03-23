using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GLFW;
using static OpenGL.Gl;

namespace CorgEng.Core.Rendering
{
    internal class RenderMaster
    {

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
            //Enable depth test -> We use z to represent the layer an object is on
            glEnable(GL_DEPTH_TEST);
            //Use the lowest number fragment
            glDepthFunc(GL_LEQUAL);
            //Enable blending for transparent objects
            glEnable(GL_BLEND);
            glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
            //Set a background colour
            glClearColor(255 / 255f, 105 / 255f, 180 / 255f, 1.0f);
        }

        /// <summary>
        /// Renders an image outputted by a render core to the screen.
        /// </summary>
        public void RenderImageToScreen(uint glImageUint)
        {
            //Reset the framebuffer (We want to draw to the screen, not a frame buffer)
            glBindFramebuffer(GL_FRAMEBUFFER, 0);
            //Clear the screen and reset it to the background colour
            glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
            //Render the image to the screen
        }

    }
}

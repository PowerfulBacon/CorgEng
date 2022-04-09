using System;
using static OpenGL.Gl;

namespace CorgEng.Core.Rendering
{
    public abstract class RenderCore
    {

        /// <summary>
        /// The uint of the frame buffer
        /// </summary>
        internal uint FrameBufferUint { get; }

        /// <summary>
        /// The uint of our render texture
        /// </summary>
        internal uint RenderTextureUint { get; }

        internal int Width { get; set; } = 1920;

        internal int Height { get; set; } = 1080;

        public unsafe RenderCore()
        {
            //Generate a frame buffer
            FrameBufferUint = glGenFramebuffer();
            glBindFramebuffer(GL_FRAMEBUFFER, FrameBufferUint);
            //Create a render texture
            glActiveTexture(GL_TEXTURE0);
            RenderTextureUint = glGenTexture();
            //Bind the created texture so we can modify it
            glBindTexture(GL_TEXTURE_2D, RenderTextureUint);
            //Load the texture scale
            glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB, Width, Height, 0, GL_RGB, GL_UNSIGNED_BYTE, NULL);
            //Set the texture parameters
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
            //Bind the framebuffer to the texture
            glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D, RenderTextureUint, 0);
            //Check for issues
            if (glCheckFramebufferStatus(GL_FRAMEBUFFER) != GL_FRAMEBUFFER_COMPLETE)
            {
                //TODO: Introduce a rendering mode that bypasses framebuffers and just draws directly to the screen.
                //Slightly broken is better than nothing.
                Console.WriteLine("WARNING: FRAMEBUFFER ERROR. Your GPU may not support this application!");
            }
        }

        internal void PreRender()
        {
            //Bind our framebuffer to render to
            glBindFramebuffer(GL_FRAMEBUFFER, FrameBufferUint);
            //Clear the backbuffer
            glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
            //Set the viewport
            glViewport(0, 0, Width, Height);
        }

        /// <summary>
        /// Called when the render core is initialized
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Perform rendering
        /// </summary>
        public abstract void PerformRender();

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OpenGL.Gl;

namespace CorgEng.Core.Rendering
{
    public abstract class RenderCore
    {

        /// <summary>
        /// The uint of the frame buffer
        /// </summary>
        internal uint FrameBufferUint { get; }

        public RenderCore()
        {
            //Generate a frame buffer
            FrameBufferUint = glGenFramebuffer();
        }

        internal void PreRender()
        {
            //Bind our framebuffer to render to
            glBindFramebuffer(FrameBufferUint);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Core
{
    public interface IRenderCore
    {

        int Width { get; }

        int Height { get; }

        /// <summary>
        /// The uint of the frame buffer
        /// </summary>
        uint FrameBufferUint { get; }

        /// <summary>
        /// The uint of our render texture
        /// </summary>
        uint RenderTextureUint { get; }

        /// <summary>
        /// Initialize the render core, run one time at the start.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Perform rendering
        /// </summary>
        void DoRender();

        /// <summary>
        /// Draws the rendered frame buffer to the screen (Framebuffer 0)
        /// </summary>
        void DrawToBuffer(uint buffer, int drawX, int drawY, int bufferWidth, int bufferHeight);

        /// <summary>
        /// Resize the render core to the specified size.
        /// </summary>
        /// <param name="width">The width in pixels of the new render core.</param>
        /// <param name="height">The height in pixels of the new render core.</param>
        void Resize(int width, int height);

    }
}

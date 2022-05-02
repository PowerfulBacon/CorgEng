using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Core
{
    public interface IRenderCore
    {

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
        void DrawToBuffer(uint buffer);

        /// <summary>
        /// Resize the render core to the specified size.
        /// </summary>
        /// <param name="width">The width in pixels of the new render core.</param>
        /// <param name="height">The height in pixels of the new render core.</param>
        void Resize(int width, int height);

    }
}

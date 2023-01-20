using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Core.Rendering
{
    public enum DepthModes
    {
        /// <summary>
        /// When the render core is drawn, everything drawn inside the render buffer
        /// will have its depth maintained, meaning that 2 render buffers will have their
        /// objects rendered with proper depth.
        /// </summary>
        KEEP_DEPTH,
        /// <summary>
        /// When the render core is drawn, depth will be ignored. If a render core
        /// is drawn onto another one, then it will be overlayed.
        /// </summary>
        IGNORE_DEPTH,
    }
}

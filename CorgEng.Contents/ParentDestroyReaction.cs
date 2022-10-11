using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Contents
{
    /// <summary>
    /// The reaction to perform when a parent is destroyed.
    /// </summary>
    public enum ParentDestroyReaction
    {
        /// <summary>
        /// Destroy all our contained children.
        /// </summary>
        DESTROY_CHILDREN,
        /// <summary>
        /// Move all the contained children to the current location of the deleted parent.
        /// </summary>
        DROP_CHILDREN,
    }
}

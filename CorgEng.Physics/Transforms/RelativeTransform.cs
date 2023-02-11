using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Physics.Transforms
{
    /// <summary>
    /// Transform based on relative positions.
    /// Does not store translation, since it assumes
    /// that it will be at the origin of some transformational
    /// space.
    /// </summary>
    internal class RelativeTransform
    {

        /// <summary>
        /// Rotational data about the physics object.
        /// Defaults to 0.
        /// </summary>
        public IVector<float> Rotation { get; set; } = new Vector<float>(0, 0, 0);

        /// <summary>
        /// The scale of the transform. Defaults to (1,1,1)
        /// </summary>
        public IVector<float> Scale { get; set; } = new Vector<float>(1, 1, 1);

    }
}

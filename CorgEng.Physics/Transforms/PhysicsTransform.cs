using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.UtilityTypes.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Physics.Transforms
{
    internal class PhysicsTransform
    {

        /// <summary>
        /// Positional data about the physics object
        /// Defaults to 0.
        /// </summary>
        public IVector<float> Position { get; set; } = new Vector<float>(0, 0, 0);

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

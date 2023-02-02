using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Physics.PhysicsBodies
{
    internal class PhysicsTransform
    {

        /// <summary>
        /// Positional data about the physics object
        /// </summary>
        public IVector<float> Position { get; set; }

        /// <summary>
        /// Rotational data about the physics object.
        /// </summary>
        public IVector<float> Rotation { get; set; }

    }
}

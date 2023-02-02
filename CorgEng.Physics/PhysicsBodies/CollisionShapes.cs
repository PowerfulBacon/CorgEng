using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Physics.PhysicsBodies
{
    public enum CollisionShapes
    {
        /// <summary>
        /// Rectangle collision shape.
        /// Uses width and height.
        /// </summary>
        COLLISION_RECT,
        /// <summary>
        /// Capsule collision shape.
        /// Uses radius and height/width.
        /// </summary>
        COLLISION_CAPSULE,
    }
}

using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.UtilityTypes.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Physics.Depreciated.Interfaces
{
    [Obsolete]
    public interface IPhysicsObject
    {

        /// <summary>
        /// Position of the physics object in the world
        /// </summary>
        IVector<float> Position { get; set; }

        /// <summary>
        /// The flags for the collision layers to use.
        /// Will collide with all physics objects where the logical OR of the 2 collision
        /// layer flags is not empty.
        /// EG Layer 1 + 2 + 4 will collide with 4 + 8 + 16
        /// But layer 8 + 16 will not collide with 1 + 2 + 4 + 32
        /// </summary>
        uint CollisionLayerFlags { get; set; }

        /// <summary>
        /// The hitbox of this physics object.
        /// </summary>
        IHitbox Hitbox { get; }

        /// <summary>
        /// Set the position of this physics object, directly moving to the target location
        /// without considering collisions in between the current and target location.
        /// </summary>
        void SetPosition(IVector<float> targetPosition);

    }
}

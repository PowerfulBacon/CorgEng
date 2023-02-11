using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.Physics.PhysicsBodies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Physics.CollisionBoxes
{
    /// <summary>
    /// Collision box.
    /// Does not support convex collision boxes.
    /// </summary>
    internal abstract class CollisionBox
    {

        /// <summary>
        /// Returns the distance that the hitbox of this object extends for
        /// in the provided direction from the origin of this collision box.
        /// 
        /// This accounts for the transform of the parent, if the parent is rotated
        /// then the collision box may have a different orientation.
        /// Note that translation is not accounted for, as the positions used
        /// are relative. The orientation of the parent object may be used, however,
        /// alone with the scale.
        /// 
        /// Other direction is the direction in which the collision box is
        /// being tested for, this should be the vector pointing towards
        /// the object that we are testing for collisions with.
        /// </summary>
        /// <returns></returns>
        public abstract float GetCollisionDistance(IVector<float> otherDirection);

        /// <summary>
        /// Get the maximum value of collision distance for any point between start and end.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public abstract float GetMaximumCollisionDistance(IVector<float> start, IVector<float> end, IVector<float> point);

    }
}

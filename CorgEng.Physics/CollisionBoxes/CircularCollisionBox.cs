using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.Physics.Transforms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Physics.CollisionBoxes
{
    internal class CircularCollisionBox : CollisionBox
    {

        /// <summary>
        /// The radius of the collision box.
        /// </summary>
        public float Radius { get; set; }

        public CircularCollisionBox(float radius)
        {
            Radius = radius;
        }

        /// <summary>
        /// The radius of a circle/sphere is constant.
        /// </summary>
        /// <param name="parentTransform"></param>
        /// <param name="otherDirection"></param>
        /// <returns></returns>
        public override float GetCollisionDistance(PhysicsTransform parentTransform, IVector<float> otherDirection)
        {
            return Radius;
        }

    }
}

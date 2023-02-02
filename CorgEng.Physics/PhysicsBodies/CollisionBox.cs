using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Physics.PhysicsBodies
{
    internal class CollisionBox
    {

        /// <summary>
        /// The shape of the collision box.
        /// </summary>
        public CollisionShapes CollisionShape { get; set; }

        private float _offsetX;
        private float _offsetY;
        private float _width;
        private float _height;

        /// <summary>
        /// The parent physics body that we are attached 
        /// </summary>
        public PhysicsBody Parent { get; }

        public CollisionBox(PhysicsBody parent)
        {

        }

        /// <summary>
        /// Radius of circular component for collision boxes.
        /// </summary>
        public float Radius
        {
            get
            {
                switch (CollisionShape)
                {
                    case CollisionShapes.COLLISION_RECT:
                        throw new ArgumentException("Cannot get the radius of a rectangular collision box.");
                    case CollisionShapes.COLLISION_CAPSULE:
                        return _width;
                }
                throw new ArgumentException("Invalid collision shapes.");
            }
            set {
                switch (CollisionShape)
                {
                    case CollisionShapes.COLLISION_RECT:
                        throw new ArgumentException("Cannot set the radius of a rectangular collision box.");
                    case CollisionShapes.COLLISION_CAPSULE:
                        _offsetX = -value;
                        _offsetY = -value;
                        _width = 2 * value;
                        _height = 2 * value;
                        break;
                }
            }
        }

        /// <summary>
        /// Horizontal size of a collision box.
        /// </summary>
        public float Width
        {
            get => _width;
            set
            {
                switch (CollisionShape)
                {
                    case CollisionShapes.COLLISION_RECT:
                        _width = value;
                        return;
                    case CollisionShapes.COLLISION_CAPSULE:
                        throw new ArgumentException("Cannot set the width of a capsule collision box.");
                }
            }
        }

        /// <summary>
        /// Vertical size of a collision box.
        /// </summary>
        public float Height
        {
            get => _height;
            set
            {
                switch (CollisionShape)
                {
                    case CollisionShapes.COLLISION_RECT:
                        _height = value;
                        return;
                    case CollisionShapes.COLLISION_CAPSULE:
                        throw new ArgumentException("Cannot set the height of a capsule collision box.");
                }
            }
        }

        public float OffsetX
        {
            get => _offsetX;
            set => _offsetX = value;
        }

        public float OffsetY
        {
            get => _offsetY;
            set => _offsetY = value;
        }

    }
}

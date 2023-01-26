using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.Physics.Depreciated.Interfaces;
using CorgEng.Physics.Depreciated.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Physics.Depreciated.PhysicsObjects
{
    [Obsolete]
    public class PhysicsObject : IPhysicsObject
    {

        private PhysicsMap currentMap;

        public IVector<float> Position { get; set; }

        public uint CollisionLayerFlags { get; set; }

        public IHitbox Hitbox { get; }

        public PhysicsObject(IVector<float> position, IHitbox hitbox)
        {
            currentMap = PhysicsManager.GetLevel(0);
            currentMap.PhysicsObjects.Add(this);
            Position = position;
            Hitbox = hitbox;
        }

        /// <summary>
        /// Move towards the specified location, being blocked by obstacles in the way.
        /// </summary>
        /// <param name="targetPosition"></param>
        public void MoveTowards(IVector<float> targetPosition)
        {
            // Identify the vector space where
            // x is perpendicular to the direction of movement
            // y is parallel to the direction of movement
            // Transform the space of the lines into this space
            // and determine which point is the closest to this object by 
        }

        /// <summary>
        /// Gets all lines that are along a line smovement path.
        /// Returns the lines at the position relative to the world.
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <returns></returns>
        public IEnumerable<ILine> GetIntersectedLinesWhenMoving(IVector<float> targetPosition)
        {
            // Get the hitbox that represents our current position
            // Get the hitbox that represents our new position
            // Locate any physics objects that we will intersect with when moving to our new location
            foreach (PhysicsObject otherObject in currentMap.PhysicsObjects)
            {
                // Don't collide with ourselves
                if (otherObject == this)
                    continue;
                // Check if we will collide with an object by moving
                foreach (ILine otherLine in otherObject.Hitbox.HitboxLines)
                {
                    ILine translatedLine = otherLine.GetTranslatedLine(otherObject.Position);
                    foreach (ILine physicsLine in Hitbox.HitboxLines)
                    {
                        if (Line.DidLineIntersect(physicsLine.GetTranslatedLine(Position), physicsLine.GetTranslatedLine(targetPosition), translatedLine))
                            yield return translatedLine;
                    }
                }
            }
        }

        public bool DidIntersectWhenMoving(IVector<float> targetPosition)
        {
            // Get the hitbox that represents our current position
            // Get the hitbox that represents our new position
            // Locate any physics objects that we will intersect with when moving to our new location
            foreach (PhysicsObject otherObject in currentMap.PhysicsObjects)
            {
                // Don't collide with ourselves
                if (otherObject == this)
                    continue;
                // Check if we will collide with an object by moving
                foreach (ILine otherLine in otherObject.Hitbox.HitboxLines)
                {
                    ILine translatedLine = otherLine.GetTranslatedLine(otherObject.Position);
                    foreach (ILine physicsLine in Hitbox.HitboxLines)
                    {
                        if (Line.DidLineIntersect(physicsLine.GetTranslatedLine(Position), physicsLine.GetTranslatedLine(targetPosition), translatedLine))
                            return true;
                    }
                }
            }
            return false;
        }

        public void SetPosition(IVector<float> targetPosition)
        {
            throw new NotImplementedException();
        }

    }
}

using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.Physics.Depreciated.Interfaces;
using CorgEng.UtilityTypes.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Physics.Depreciated.PhysicsObjects
{
    [Obsolete]
    public class Line : ILine
    {

        public IVector<float> Start { get; set; }

        public IVector<float> End { get; set; }

        public Line(IVector<float> start, IVector<float> end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        /// Gives the intersection point on the line other.
        /// If the 2 lines are parallel, then
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public float? IntersectionPoint(ILine other)
        {
            // Setup the variables for convenience
            float x1 = Start.X;
            float y1 = Start.Y;
            float x2 = End.X;
            float y2 = End.Y;
            float a1 = other.Start.X;
            float b1 = other.Start.Y;
            float a2 = other.End.X;
            float b2 = other.End.Y;
            // Calculate lambda
            //float lambda = ((x1 - a1) * (b2 - b1) - (y2 - b1) * (a2 - a1)) / ((y2 - y1) * (a2 - a1) + (x1 - x2) * (b2 - b1));
            // Optimisation
            float b2mb1 = b2 - b1;
            float a2ma1 = a2 - a1;
            float x2mx1 = x2 - x1;
            float y2my1 = y2 - y1;

            float lambdaComponent = b2mb1 * x2mx1 - y2my1 * a2ma1;
            //Try it this way
            if (lambdaComponent != 0)
                return ((a1 - x1) * b2mb1 + (y1 - b1) * a2ma1) / lambdaComponent;
            //Try it the alternative way
            lambdaComponent = a2ma1 * y2my1 - x2mx1 * b2mb1;
            if (lambdaComponent != 0)
                return ((b1 - y1) * a2ma1 + (x1 - a1) * b2mb1) / lambdaComponent;
            //Parallel
            return null;
        }


        /// <summary>
        /// Determines if a line transforming from start to end intersected with test
        /// during its transformation.
        /// Returns null if no intersection occurred, otherwise returns the
        /// point of intersection relative to the input space.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="test"></param>
        /// <returns></returns>
        public static bool DidLineIntersect(ILine start, ILine end, ILine test)
        {
            ILine toStart = new Line(start.Start, end.Start);
            ILine toEnd = new Line(start.End, end.End);
            float? topMu = toStart.IntersectionPoint(test);
            float? bottomMu = toEnd.IntersectionPoint(test);
            if (topMu == null || bottomMu == null)
            {
                //Either one is parallel
                //Treat the points on the test line like individual points
            }
            // Direct intersection
            if (topMu >= 0 && topMu <= 1)
                return true;
            if (bottomMu >= 0 && bottomMu <= 1)
                return true;
            // If they intersected on different sides, then we passed through the object
            if (topMu > 1 && bottomMu < 0)
                return true;
            if (topMu < 0 && bottomMu > 1)
                return true;
            return false;
        }

        public ILine GetTranslatedLine(IVector<float> position)
        {
            return new Line((Vector<float>)Start + position, (Vector<float>)End + position);
        }

        /// <summary>
        /// Transforms the space of the transform.
        /// </summary>
        /// <param name="spaceX"></param>
        /// <param name="spaceY"></param>
        /// <returns></returns>
        public ILine TransformSpace(IVector<float> spaceX, IVector<float> spaceY)
        {
            //X space transformation
            float a = spaceX.X;
            float c = spaceX.Y;
            //Y space transformation
            float b = spaceY.X;
            float d = spaceY.Y;
            //Create the transformed line
            return new Line(
                new Vector<float>(Start.X * a + Start.Y * b, Start.X * c + Start.Y * d),
                new Vector<float>(End.X * a + End.Y * b, End.X * c + End.Y * d)
                );
        }
    }
}

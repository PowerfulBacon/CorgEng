using CorgEng.Core.Dependencies;
using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.GenericInterfaces.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.UtilityTypes.Linecasting
{
    [Dependency]
    internal class LineCaster : ILineCaster
    {

        private const float VERY_SMALL_VALUE = 0.00001f;

        [UsingDependency]
        private static IEntityPositionTracker World = null!;

        /// <summary>
        /// Lazilly evaluated enumerable of locations between start and end.
        /// Can be used with linq to determine sightlines.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public IEnumerable<IVector<int>> GetTilesBetween(IVector<int> start, IVector<int> end)
        {
            if (start == end)
            {
                yield return start;
                yield break;
            }
            double deltaX = end.X - start.X;
            double deltaY = end.Y - start.Y;
            double deltaLength = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            deltaX /= deltaLength;
            deltaY /= deltaLength;
            IVector<float> currentPosition = start.CastCopy<float>();
            double distanceLeft = start.DistanceTo(end);
            while (distanceLeft >= 0)
            {
                //Calculate how much x,y,z until we reach the next int
                //-4.5 (-5)
                //-4 -1 +4.5 = -5 + 4.5 = -0.5
                double xLeft = GetAppropriateRounding(currentPosition[0], deltaX) + Math.Sign(deltaX) - currentPosition[0];
                double yLeft = GetAppropriateRounding(currentPosition[1], deltaY) + Math.Sign(deltaY) - currentPosition[1];
                //Calculate how many steps we need of each delta to reach it
                //In this case division by 0 should represent infinity
                double xStep = deltaX == 0 ? double.PositiveInfinity : xLeft / deltaX;
                double yStep = deltaY == 0 ? double.PositiveInfinity : yLeft / deltaY;
                //Choose the smallest one and move there
                double smallest = Math.Min(xStep, yStep);
                distanceLeft -= smallest;
                //Move the raycaster forward
                currentPosition.X += (float)(deltaX * (smallest + VERY_SMALL_VALUE));
                currentPosition.Y += (float)(deltaY * (smallest + VERY_SMALL_VALUE));
                //Get the block at the current psoition and test raycast hit
                yield return World.GetGridPosition(currentPosition);
            }
        }

        public IEnumerable<IVector<int>> GetTilesBetween(IVector<float> start, IVector<float> end)
        {
            return GetTilesBetween(World.GetGridPosition(start), World.GetGridPosition(end));
        }

        private static double GetAppropriateRounding(double position, double delta)
        {
            if (Math.Sign(position) == Math.Sign(delta))
            {
                return (int)position;
            }
            else if (position > 0)
            {
                return Math.Ceiling(position);
            }
            else
            {
                return Math.Floor(position);
            }
        }

    }
}

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
    public interface ILine
    {

        /// <summary>
        /// Vector position of the start of the line
        /// </summary>
        IVector<float> Start { get; set; }

        /// <summary>
        /// Vector position of the end of the line
        /// </summary>
        IVector<float> End { get; set; }

        /// <summary>
        /// Returns the point of the interaction with another line.
        /// This represents the position along this line where the intersection occured.
        /// 
        /// Returns null if the 2 lines are parallel and will never intersect.
        /// If the returned value is between 0 and 1, then the intersection occures within the other line.
        /// If the returned value is greater than 1, then the point of intersection is past the end of the other line.
        /// If the returned value is less than 0, then the point of intersection is before the other line.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        float? IntersectionPoint(ILine other);

        /// <summary>
        /// Translate the line
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        ILine GetTranslatedLine(IVector<float> position);

        /// <summary>
        /// Transform the space from
        /// x: (0,0) -> (1,0)
        /// y: (0,0) -> (0, 1)
        /// to the space
        /// x: (0,0) -> spaceX
        /// y: (0,0) -> spaceY
        /// </summary>
        /// <returns>Returns the vector in the transformed space</returns>
        ILine TransformSpace(IVector<float> spaceX, IVector<float> spaceY);

    }
}

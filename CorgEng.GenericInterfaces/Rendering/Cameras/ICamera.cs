using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Rendering
{
    public interface ICamera
    {

        IMatrix GetProjectionMatrix(float windowWidth, float windowHeight);

        IMatrix GetViewMatrix(float windowWidth, float windowHeight);

        /// <summary>
        /// Converts world coordinates into screen coordinates.
        /// </summary>
        /// <param name="x">The X position ranging from -1 to 1</param>
        /// <param name="y">The Y position ranging from -1 to 1</param>
        /// <returns></returns>
        IVector<float> ScreenToWorldCoordinates(double x, double y, float windowWidth, float windowHeight);

    }
}

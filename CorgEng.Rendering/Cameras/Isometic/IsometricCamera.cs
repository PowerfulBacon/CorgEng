using CorgEng.GenericInterfaces.Rendering;
using CorgEng.GenericInterfaces.Rendering.Cameras.Isometric;
using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.UtilityTypes.Matrices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Rendering.Cameras.Isometic
{
    public class IsometricCamera : IIsometricCamera
    {

        public float X { get; set; }

        public float Y { get; set; }

        public float Width { get; set; }

        public float Height { get; set; }

        public IMatrix GetProjectionMatrix(float windowWidth, float windowHeight)
        {
            return Matrix.GetScaleMatrix(windowHeight / windowWidth, 1.0f, 0.01f);
        }

        public IMatrix GetViewMatrix(float windowWidth, float windowHeight)
        {
            return Matrix.Identity[4];
        }

    }
}

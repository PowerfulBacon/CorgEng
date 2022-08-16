using CorgEng.GenericInterfaces.Rendering;
using CorgEng.GenericInterfaces.Rendering.Cameras.Isometric;
using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.UtilityTypes.Matrices;
using CorgEng.UtilityTypes.Vectors;
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

        public float Width { get; set; } = 2;

        public float Height { get; set; } = 2;

        public IMatrix GetProjectionMatrix(float windowWidth, float windowHeight)
        {
            return Matrix.GetScaleMatrix(windowHeight / windowWidth, 1.0f, 0.01f);
        }

        public IMatrix GetViewMatrix(float windowWidth, float windowHeight)
        {
            return Matrix.GetScaleMatrix(1.0f / (Width * 0.5f), 1.0f / (Height * 0.5f), 1.0f) * Matrix.GetTranslationMatrix(-X, -Y, 0);
        }

        public IVector<float> ScreenToWorldCoordinates(double x, double y, float windowWidth, float windowHeight)
        {
            float widthMultiplier = (windowHeight / windowWidth) * (1.0f / (Width * 0.5f));
            float heightMultiplier = 1.0f / (Height * 0.5f);
            //Scale the X and Y
            float worldX = (float)(x / widthMultiplier) + X;
            float worldY = (float)(y / heightMultiplier) - Y;
            return new Vector<float>(worldX, -worldY);
        }
    }
}

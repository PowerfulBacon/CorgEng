using CorgEng.GenericInterfaces.Rendering.Textures;

namespace CorgEng.Rendering.Textures
{

    internal class TextureJson
    {

        public TextureJson(string fileName, int width, int height, int indexX, int indexY, DirectionalModes directionalModes = DirectionalModes.NONE)
        {
            FileName = fileName;
            Width = width;
            Height = height;
            IndexX = indexX;
            IndexY = indexY;
            DirectionalModes = directionalModes;
        }

        public string FileName { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int IndexX { get; set; }

        public int IndexY { get; set; }

        public DirectionalModes DirectionalModes { get; set; }

    }
}

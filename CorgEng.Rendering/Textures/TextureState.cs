using CorgEng.GenericInterfaces.Rendering.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Rendering.Textures
{
    public class TextureState : ITextureState
    {

        public ITexture TextureFile { get; private set; }

        public double OffsetX { get; private set; }

        public double OffsetY { get; private set; }

        public double OffsetWidth { get; private set; }

        public double OffsetHeight { get; private set; }

        public TextureState(ITexture textureFile, double offsetX, double offsetY, double offsetWidth, double offsetHeight)
        {
            TextureFile = textureFile;
            OffsetX = offsetX;
            OffsetY = offsetY;
            OffsetWidth = offsetWidth;
            OffsetHeight = offsetHeight;
        }
    }
}

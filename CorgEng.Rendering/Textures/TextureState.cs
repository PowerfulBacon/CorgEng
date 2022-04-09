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

        public float OffsetX { get; private set; }

        public float OffsetY { get; private set; }

        public float OffsetWidth { get; private set; }

        public float OffsetHeight { get; private set; }

        public TextureState(ITexture textureFile, float offsetX, float offsetY, float offsetWidth, float offsetHeight)
        {
            TextureFile = textureFile;
            OffsetX = offsetX;
            OffsetY = offsetY;
            OffsetWidth = offsetWidth;
            OffsetHeight = offsetHeight;
        }
    }
}

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

        private enum TransparentState
        {
            Undetermined,
            Transparent,
            NotTransparent,
        }

        public ITexture TextureFile { get; private set; }

        public float OffsetX { get; private set; }

        public float OffsetY { get; private set; }

        public float OffsetWidth { get; private set; }

        public float OffsetHeight { get; private set; }

        private TransparentState _isTransparentCache;

        public bool IsTransparentTexture
        {
            get
            {
                //determine transparent state
                if (_isTransparentCache == TransparentState.Undetermined)
                {
                    _isTransparentCache = TextureFile.IsTextureTransparent(this) ? TransparentState.Transparent : TransparentState.NotTransparent;
                }
                //Fetch transparency
                return _isTransparentCache == TransparentState.Transparent;
            }
        }

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

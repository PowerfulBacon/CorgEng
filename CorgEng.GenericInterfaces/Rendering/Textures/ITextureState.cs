using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Rendering.Textures
{
    public interface ITextureState
    {

        ITexture TextureFile { get; }

        float OffsetX { get; }

        float OffsetY { get; }

        float OffsetWidth { get; }

        float OffsetHeight { get; }

    }
}

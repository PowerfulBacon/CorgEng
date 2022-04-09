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

        double OffsetX { get; }

        double OffsetY { get; }

        double OffsetWidth { get; }

        double OffsetHeight { get; }

    }
}

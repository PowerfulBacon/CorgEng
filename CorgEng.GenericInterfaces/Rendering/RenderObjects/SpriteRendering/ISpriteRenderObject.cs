using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Rendering.RenderObjects.SpriteRendering
{
    public interface ISpriteRenderObject : IRenderObject
    {

        IBindableProperty<uint> TextureFile { get; }

        IBindableProperty<double> TextureFileX { get; }
        IBindableProperty<double> TextureFileY { get; }

        IBindableProperty<double> TextureFileHeight { get; }
        IBindableProperty<double> TextureFileWidth { get; }

    }
}

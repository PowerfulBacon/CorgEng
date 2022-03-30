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

        IBindableProperty<float> TextureFileX { get; }
        IBindableProperty<float> TextureFileY { get; }

        IBindableProperty<float> TextureFileHeight { get; }
        IBindableProperty<float> TextureFileWidth { get; }

        IBindablePropertyGroup TextureDetails { get; }

    }
}

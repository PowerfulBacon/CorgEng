using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.Rendering.Positioning;
using CorgEng.GenericInterfaces.Rendering.RenderObjects.SpriteRendering;
using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Rendering.SpriteRendering
{
    public sealed class SpriteRenderObject : ISpriteRenderObject
    {
        public IBindableProperty<uint> TextureFile => throw new NotImplementedException();

        public IBindableProperty<double> TextureFileX => throw new NotImplementedException();

        public IBindableProperty<double> TextureFileY => throw new NotImplementedException();

        public IBindableProperty<double> TextureFileHeight => throw new NotImplementedException();

        public IBindableProperty<double> TextureFileWidth => throw new NotImplementedException();

        public IWorldPosition WorldPosition => throw new NotImplementedException();
    }
}

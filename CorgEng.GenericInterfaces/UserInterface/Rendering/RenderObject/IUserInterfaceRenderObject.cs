using CorgEng.GenericInterfaces.Rendering.RenderObjects;
using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.UserInterface.Rendering.RenderObject
{
    public interface IUserInterfaceRenderObject : IRenderObject
    {

        IBindableProperty<uint> TextureFile { get; set; }

        IBindableProperty<float> TextureFileX { get; set; }
        IBindableProperty<float> TextureFileY { get; set; }

        IBindableProperty<float> TextureFileHeight { get; set; }
        IBindableProperty<float> TextureFileWidth { get; set; }

        IBindablePropertyGroup TextureDetails { get; }

    }
}

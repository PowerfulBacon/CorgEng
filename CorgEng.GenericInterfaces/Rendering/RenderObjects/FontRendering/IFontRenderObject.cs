using CorgEng.GenericInterfaces.Font.Fonts;
using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Rendering.RenderObjects.FontRendering
{
    public interface IFontRenderObject : IRenderObject
    {

        IBindableProperty<string> Text { get; }

        IBindableProperty<IFont> FontProperty { get; }

    }
}

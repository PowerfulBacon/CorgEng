using CorgEng.GenericInterfaces.Font.Fonts;
using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.UtilityTypes.BindableProperties;
using CorgEng.UtilityTypes.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Rendering.FontRendering
{
    internal sealed class FontRenderObject
    {

        public IBindableProperty<string> Text { get; }

        public IBindableProperty<IFont> FontProperty { get; }

        public IBindableProperty<IVector<float>> ScreenPosition { get; } = new BindableProperty<IVector<float>>(new Vector<float>(0, 0, 0));

    }
}

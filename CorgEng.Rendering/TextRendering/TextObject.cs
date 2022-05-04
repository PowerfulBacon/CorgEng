using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.Rendering.Renderers.SpriteRendering;
using CorgEng.GenericInterfaces.Rendering.Text;
using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.UtilityTypes.BindableProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Rendering.TextRendering
{
    internal class TextObject : ITextObject
    {

        public IBindableProperty<string> TextProperty { get; }

        //TODO
        public IBindableProperty<IVector<float>> ScreenPositionProperty { get; }

        //Collect a sprite renderer dependency so that we can draw our glyphs.
        [UsingDependency]
        private static ISpriteRenderer SpriteRenderer;

        public TextObject(string text)
        {
            //Create the text property
            TextProperty = new BindableProperty<string>(text);
        }

        public void StartRendering()
        {
            
        }

        public void StopRendering()
        {
            throw new NotImplementedException();
        }

    }
}

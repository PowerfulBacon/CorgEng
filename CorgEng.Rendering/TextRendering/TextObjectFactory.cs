using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.Font.Fonts;
using CorgEng.GenericInterfaces.Rendering.Renderers.SpriteRendering;
using CorgEng.GenericInterfaces.Rendering.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Rendering.TextRendering
{
    [Dependency]
    internal class TextObjectFactory : ITextObjectFactory
    {

        public ITextObject CreateTextObject(ISpriteRenderer renderer, IFont font, string text)
        {
            return new TextObject(renderer, font, text);
        }

    }
}

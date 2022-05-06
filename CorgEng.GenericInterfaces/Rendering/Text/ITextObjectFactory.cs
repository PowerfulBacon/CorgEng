using CorgEng.GenericInterfaces.Font.Fonts;
using CorgEng.GenericInterfaces.Rendering.Renderers.SpriteRendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Rendering.Text
{
    public interface ITextObjectFactory
    {

        /// <summary>
        /// Create a text object
        /// </summary>
        ITextObject CreateTextObject(ISpriteRenderer renderer, IFont font, string text);

    }
}

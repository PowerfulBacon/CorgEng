using CorgEng.GenericInterfaces.Rendering.Renderers.FontRendering;
using CorgEng.GenericInterfaces.Rendering.RenderObjects.FontRendering;
using CorgEng.Rendering.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Rendering.FontRendering
{
    internal class FontRenderer : InstancedRenderer<FontSharedRenderAttributes, FontBatch>, IFontRenderer
    {

        public void StartRendering(IFontRenderObject fontRenderObject)
        {
            //Check for duplicate render exceptions
            if (fontRenderObject.GetBelongingBatchElement<FontBatch>() != null)
                throw new DuplicateRenderException("Attempting to render a text object already being rendered.");
            //Create a new batch element
            //Register listeners for text changing
            fontRenderObject.Text.ValueChanged += OnTextChanged;
        }

        public void StopRendering(IFontRenderObject fontRenderObject)
        {
            throw new NotImplementedException();
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            //SOOOoo
        }

    }
}

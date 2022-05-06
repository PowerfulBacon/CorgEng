using CorgEng.UtilityTypes.Batches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Rendering.FontRendering
{
    internal sealed class FontBatch : Batch<FontBatch>
    {

        public override int[] BatchVectorSizes => new int[] { 3, 4 };

    }
}

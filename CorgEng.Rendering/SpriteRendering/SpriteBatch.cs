using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.GenericInterfaces.UtilityTypes.Batches;
using CorgEng.UtilityTypes.Batches;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Rendering.SpriteRendering
{
    internal sealed class SpriteBatch : Batch<SpriteBatch>
    {

        public override int[] BatchVectorSizes { get; } = new int[] { 3, 3, 4, 1 };

    }
}

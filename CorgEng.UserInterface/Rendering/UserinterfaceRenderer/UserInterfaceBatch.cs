using CorgEng.UtilityTypes.Batches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.UserInterface.Rendering.UserinterfaceRenderer
{
    internal sealed class UserInterfaceBatch : Batch<UserInterfaceBatch>
    {

        public override int[] BatchVectorSizes { get; } = new int[] { 3, 3, 4 };

    }
}

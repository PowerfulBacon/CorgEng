using CorgEng.GenericInterfaces.Rendering.Positioning;
using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Rendering.RenderObjects
{
    public interface IRenderObject
    {

        IWorldPosition WorldPosition { get; }

    }
}

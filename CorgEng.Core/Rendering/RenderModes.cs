using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Core.Rendering
{
    public enum RenderModes
    {
        //Default blend mode as*(1-a)d
        DEFAULT,
        //Multiplicative blend mode ds
        MULTIPLY,
        //Additive blend mode as+ad
        ADDITIVE,
    }
}

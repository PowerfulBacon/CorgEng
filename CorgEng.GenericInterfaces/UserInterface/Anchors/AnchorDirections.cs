using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.UserInterface.Anchors
{
    public enum AnchorDirections
    {
        TOP = 1 << 0,
        RIGHT = 1 << 1,
        BOTTOM = 1 << 2,
        LEFT = 1 << 3,
    }
}

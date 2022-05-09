using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.UserInterface.Anchors
{
    public interface IAnchor
    {

        IAnchorDetails TopDetails { get; }

        IAnchorDetails RightDetails { get; }

        IAnchorDetails BottomDetails { get; }

        IAnchorDetails LeftDetails { get; }

    }
}

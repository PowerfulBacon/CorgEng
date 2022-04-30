using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.UserInterface.Anchors
{
    public interface IAnchorDetailFactory
    {

        IAnchorDetails CreateAnchorDetails(
            AnchorDirections anchorSide,
            AnchorUnits anchorUnits,
            double anchorOffset,
            bool strict = false
        );

    }
}

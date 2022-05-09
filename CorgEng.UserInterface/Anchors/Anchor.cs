using CorgEng.GenericInterfaces.UserInterface.Anchors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.UserInterface.Anchors
{
    internal class Anchor : IAnchor
    {

        public IAnchorDetails TopDetails { get; }

        public IAnchorDetails RightDetails { get; }

        public IAnchorDetails BottomDetails { get; }

        public IAnchorDetails LeftDetails { get; }

        public Anchor()
        {
            TopDetails = new AnchorDetails(AnchorDirections.TOP, AnchorUnits.PIXELS, 0);
            RightDetails = new AnchorDetails(AnchorDirections.RIGHT, AnchorUnits.PIXELS, 0);
            BottomDetails = new AnchorDetails(AnchorDirections.BOTTOM, AnchorUnits.PIXELS, 0);
            LeftDetails = new AnchorDetails(AnchorDirections.LEFT, AnchorUnits.PIXELS, 0);
        }

        public Anchor(IAnchorDetails topDetails, IAnchorDetails rightDetails, IAnchorDetails bottomDetails, IAnchorDetails leftDetails)
        {
            TopDetails = topDetails;
            RightDetails = rightDetails;
            BottomDetails = bottomDetails;
            LeftDetails = leftDetails;
        }
    }
}

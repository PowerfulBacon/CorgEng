using CorgEng.GenericInterfaces.UserInterface.Anchors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.UserInterface.Anchors
{
    internal class AnchorDetails : IAnchorDetails
    {

        public AnchorDirections AnchorSide { get; }

        public AnchorUnits AnchorUnits { get; }

        public double AnchorOffset { get; } = 0;

        public bool Strict { get; } = false;

        public AnchorDetails(AnchorDirections anchorSide, AnchorUnits anchorUnits, double anchorOffset, bool strict = false)
        {
            AnchorSide = anchorSide;
            AnchorUnits = anchorUnits;
            AnchorOffset = anchorOffset;
            Strict = strict;
        }
    }
}

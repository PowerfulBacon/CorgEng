using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.UserInterface.Anchors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.UserInterface.Anchors
{
    [Dependency]
    internal class AnchorDetailFactory : IAnchorDetailFactory
    {

        public IAnchorDetails CreateAnchorDetails(AnchorDirections anchorSide, AnchorUnits anchorUnits, double anchorOffset, bool strict = false)
        {
            return new AnchorDetails(anchorSide, anchorUnits, anchorOffset, strict);
        }

    }
}

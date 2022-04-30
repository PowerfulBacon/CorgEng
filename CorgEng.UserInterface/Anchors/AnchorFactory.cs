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
    internal class AnchorFactory : IAnchorFactory
    {

        public IAnchor CreateAnchor(IAnchorDetails leftAnchorDetails, IAnchorDetails rightAnchorDetails, IAnchorDetails topAnchorDetails, IAnchorDetails bottomAnchorDetails)
        {
            return new Anchor(topAnchorDetails, rightAnchorDetails, bottomAnchorDetails, leftAnchorDetails);
        }

    }
}

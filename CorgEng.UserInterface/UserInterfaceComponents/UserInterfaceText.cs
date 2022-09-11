using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.Font.Fonts;
using CorgEng.GenericInterfaces.Rendering.Text;
using CorgEng.GenericInterfaces.UserInterface.Anchors;
using CorgEng.GenericInterfaces.UserInterface.Components;
using CorgEng.UserInterface.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.UserInterface.UserInterfaceComponents
{
    internal class UserInterfaceText : UserInterfaceComponent
    {

        [UsingDependency]
        private static IFontFactory FontFactory;

        [UsingDependency]
        private static ITextObjectFactory TextObjectFactory;

        public UserInterfaceText(IAnchor anchorDetails, IDictionary<string, string> arguments) : base(anchorDetails, arguments)
        { }

        public UserInterfaceText(IUserInterfaceComponent parent, IAnchor anchorDetails, IDictionary<string, string> arguments) : base(parent, anchorDetails, arguments)
        { }



    }
}

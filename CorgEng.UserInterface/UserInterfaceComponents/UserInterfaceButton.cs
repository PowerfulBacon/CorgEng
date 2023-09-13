using CorgEng.Core;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.UserInterface.Anchors;
using CorgEng.GenericInterfaces.UserInterface.Components;
using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.UserInterface.Rendering.UserinterfaceRenderer.Box;
using CorgEng.UtilityTypes.Colours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.UserInterface.Components
{
    internal class UserInterfaceButton : UserInterfaceBox
    {

        public UserInterfaceButton(IWorld world, IUserInterfaceComponent parent, IAnchor anchorDetails, IDictionary<string, string> arguments) : base(world, parent, anchorDetails, arguments)
        {
        }

        public UserInterfaceButton(IWorld world, IAnchor anchorDetails, IDictionary<string, string> arguments) : base(world, anchorDetails, arguments)
        {
        }

        private UserInterfaceBoxRenderer userInterfaceBoxRenderer;

    }
}

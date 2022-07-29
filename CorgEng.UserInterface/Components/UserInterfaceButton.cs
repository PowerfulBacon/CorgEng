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
    internal class UserInterfaceButton : UserInterfaceComponent
    {

        public UserInterfaceButton(IUserInterfaceComponent parent, IAnchor anchorDetails, IDictionary<string, string> arguments) : base(parent, anchorDetails)
        {
            Setup(arguments);
        }

        public UserInterfaceButton(IAnchor anchorDetails, IDictionary<string, string> arguments) : base(anchorDetails)
        {
            Setup(arguments);
        }

        private void Setup(IDictionary<string, string> arguments)
        {
        }

        private UserInterfaceBoxRenderer userInterfaceBoxRenderer;

        public override void Initialize()
        {
            userInterfaceBoxRenderer = new UserInterfaceBoxRenderer();
            userInterfaceBoxRenderer.Initialize();
        }

        public override void PerformRender()
        {
            userInterfaceBoxRenderer.Render(Width, Height);
        }

    }
}

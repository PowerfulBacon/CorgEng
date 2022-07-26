using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.UserInterface.Anchors;
using CorgEng.GenericInterfaces.UserInterface.Components;
using CorgEng.UserInterface.Rendering.UserinterfaceRenderer.Box;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.UserInterface.Components
{
    internal class UserInterfaceBox : UserInterfaceComponent
    {

        public UserInterfaceBox(IUserInterfaceComponent parent, IAnchor anchorDetails) : base(parent, anchorDetails)
        {
        }

        public UserInterfaceBox(IAnchor anchorDetails) : base(anchorDetails)
        {
        }

        private UserInterfaceBoxRenderer userInterfaceBoxRenderer;

        public override void Initialize()
        {
            userInterfaceBoxRenderer = new UserInterfaceBoxRenderer();
            userInterfaceBoxRenderer.Initialize();
            //Render a box
            UserInterfaceBoxRenderObject boxRenderObject = new UserInterfaceBoxRenderObject();
            userInterfaceBoxRenderer.StartRendering(boxRenderObject);
        }

        public override void PerformRender()
        {
            userInterfaceBoxRenderer.Render(Width, Height);
        }
    }
}

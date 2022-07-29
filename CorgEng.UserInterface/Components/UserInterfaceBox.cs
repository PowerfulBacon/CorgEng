using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.UserInterface.Anchors;
using CorgEng.GenericInterfaces.UserInterface.Components;
using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.UserInterface.Rendering.UserinterfaceRenderer.Box;
using CorgEng.UserInterface.Style;
using CorgEng.UtilityTypes.Colours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.UserInterface.Components
{
    internal class UserInterfaceBox : UserInterfaceComponent
    {

        /// <summary>
        /// The render object.
        /// </summary>
        private UserInterfaceBoxRenderObject boxRenderObject = new UserInterfaceBoxRenderObject();

        /// <summary>
        /// Helper property to get the style component of our box
        /// </summary>
        public BoxStyle Style
        {
            get => boxRenderObject.Style;
        }

        public UserInterfaceBox(IUserInterfaceComponent parent, IAnchor anchorDetails, IDictionary<string, string> arguments) : base(parent, anchorDetails)
        {
            Setup(arguments);
        }

        public UserInterfaceBox(IAnchor anchorDetails, IDictionary<string, string> arguments) : base(anchorDetails)
        {
            Setup(arguments);
        }

        /// <summary>
        /// Setup the component by parsing the styles and starting to render the component
        /// </summary>
        /// <param name="arguments"></param>
        private void Setup(IDictionary<string, string> arguments)
        {
            Style.ParseSettings(arguments);
            userInterfaceBoxRenderer.StartRendering(boxRenderObject);
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

using CorgEng.Core;
using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.EntityComponentSystem;
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

        public UserInterfaceBox(IWorld world, IUserInterfaceComponent parent, IAnchor anchorDetails, IDictionary<string, string> arguments) : base(world, parent, anchorDetails, arguments)
        {
            Setup(arguments);
        }

        public UserInterfaceBox(IWorld world, IAnchor anchorDetails, IDictionary<string, string> arguments) : base(world, anchorDetails, arguments)
        {
            Setup(arguments);
        }

        /// <summary>
        /// Setup the component by parsing the styles and starting to render the component
        /// </summary>
        /// <param name="arguments"></param>
        private void Setup(IDictionary<string, string> arguments)
        {
            //Parse
            Style.ParseSettings(arguments);
        }

        private UserInterfaceBoxRenderer userInterfaceBoxRenderer;

        public override void Initialize()
        {
            base.Initialize();
            if (!CorgEngMain.IsRendering)
                return;
            userInterfaceBoxRenderer = new UserInterfaceBoxRenderer();
            userInterfaceBoxRenderer.Initialize();
            //Start rendering
            if (CorgEngMain.IsRendering)
                userInterfaceBoxRenderer.StartRendering(boxRenderObject);
        }

        public override void PerformRender()
        {
            if (!CorgEngMain.IsRendering)
                return;
            userInterfaceBoxRenderer.Render(Width, Height);
        }
    }
}

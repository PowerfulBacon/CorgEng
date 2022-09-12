using CorgEng.Core;
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

        public UserInterfaceBox(IUserInterfaceComponent parent, IAnchor anchorDetails, IDictionary<string, string> arguments) : base(parent, anchorDetails, arguments)
        {
            Setup(arguments);
        }

        public UserInterfaceBox(IAnchor anchorDetails, IDictionary<string, string> arguments) : base(anchorDetails, arguments)
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
            //Start rendering
            if (CorgEngMain.IsRendering)
                userInterfaceBoxRenderer.StartRendering(boxRenderObject);
            //Add on components
            if (arguments.ContainsKey("onClick"))
            {
                string methodName = arguments["onClick"];
                if (!UserInterfaceModule.KeyMethods.ContainsKey(methodName))
                {
                    throw new Exception($"No static method with the key '{methodName}' exists.");
                }
                ComponentHolder.AddComponent(new UserInterfaceClickerComponent(UserInterfaceModule.KeyMethods[methodName]));
            }
        }

        private UserInterfaceBoxRenderer userInterfaceBoxRenderer;

        public override void Initialize()
        {
            if (!CorgEngMain.IsRendering)
                return;
            userInterfaceBoxRenderer = new UserInterfaceBoxRenderer();
            userInterfaceBoxRenderer.Initialize();
        }

        public override void PerformRender()
        {
            if (!CorgEngMain.IsRendering)
                return;
            userInterfaceBoxRenderer.Render(Width, Height);
        }
    }
}

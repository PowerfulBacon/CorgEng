using CorgEng.Constants;
using CorgEng.Core;
using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Rendering;
using CorgEng.GenericInterfaces.Rendering.Icons;
using CorgEng.GenericInterfaces.Rendering.Textures;
using CorgEng.GenericInterfaces.UserInterface.Anchors;
using CorgEng.GenericInterfaces.UserInterface.Components;
using CorgEng.UserInterface.Components;
using CorgEng.UserInterface.Rendering.UserinterfaceRenderer.Icon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.UserInterface.UserInterfaceComponents
{
    internal class UserInterfaceIcon : UserInterfaceComponent
    {

        [UsingDependency]
        private static IIconFactory IconFactory;

        [UsingDependency]
        private static ITextureFactory TextureFactory;

        [UsingDependency]
        private static ILogger Logger;

        private UserInterfaceIconRenderObject userInterfaceIconRenderObject;

        private UserInterfaceIconRenderer userInterfaceIconRenderer;

        /// <summary>
        /// Not included in screencasting for now
        /// </summary>
        protected override bool ScreencastInclude { get; } = false;

        public UserInterfaceIcon(IAnchor anchorDetails, IDictionary<string, string> arguments) : base(anchorDetails, arguments)
        {
            Init(arguments);
        }

        public UserInterfaceIcon(IUserInterfaceComponent parent, IAnchor anchorDetails, IDictionary<string, string> arguments) : base(parent, anchorDetails, arguments)
        {
            Init(arguments);
        }

        private void Init(IDictionary<string, string> arguments)
        {
            try
            {
                userInterfaceIconRenderObject = new UserInterfaceIconRenderObject(TextureFactory.GetTextureFromIconState(IconFactory.CreateIcon(arguments["icon"], 0, RenderingConstants.DEFAULT_RENDERER_PLANE)));
            }
            catch (Exception e)
            {
                Logger.WriteLine($"Failed to create user interface icon. This is most likely due to not implement the icon argument.\n{e}", LogType.ERROR);
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            if (!CorgEngMain.IsRendering)
                return;
            userInterfaceIconRenderer = new UserInterfaceIconRenderer();
            userInterfaceIconRenderer.Initialize();
            //Start rendering
            if (CorgEngMain.IsRendering)
                userInterfaceIconRenderer.StartRendering(userInterfaceIconRenderObject);
        }

		public override void Render(ICamera camera)
		{
            if (!CorgEngMain.IsRendering)
                return;
            userInterfaceIconRenderer.Render(Width, Height);
        }

    }
}

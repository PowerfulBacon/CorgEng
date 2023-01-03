using CorgEng.Core;
using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.Font.Fonts;
using CorgEng.GenericInterfaces.Rendering.Cameras.Isometric;
using CorgEng.GenericInterfaces.Rendering.Renderers.SpriteRendering;
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

        [UsingDependency]
        private static ISpriteRendererFactory SpriteRendererFactory;

        [UsingDependency]
        private static IIsometricCameraFactory IsometricCameraFactory;

        private IIsometricCamera IsometricCamera;

        public ISpriteRenderer spriteRenderer;

        private ITextObject internalTextObject;

        protected override bool ScreencastInclude => false;

        private bool ready = false;

        public string Text { get; private set; } = "";

        private IFont font;

        public UserInterfaceText(IAnchor anchorDetails, IDictionary<string, string> arguments) : base(anchorDetails, arguments)
        {
            Setup(arguments);
        }

        public UserInterfaceText(IUserInterfaceComponent parent, IAnchor anchorDetails, IDictionary<string, string> arguments) : base(parent, anchorDetails, arguments)
        {
            Setup(arguments);
        }

        private void Setup(IDictionary<string, string> arguments)
        {
            //Create the camera
            IsometricCamera = IsometricCameraFactory.CreateCamera();
            //Create the renderer (Local, since UI)
            spriteRenderer = SpriteRendererFactory.CreateSpriteRenderer(Constants.NetworkedRenderingConstants.NETWORK_RENDERING_ID_LOCAL);
            spriteRenderer.Initialize();
            //Get the font to use
            string typeface;
            arguments.TryGetValue("font", out typeface);
            //Create / Get the font
            font = FontFactory.GetFont(typeface ?? "CourierCode");
            //Create a text object
            string text = arguments["text"];
            internalTextObject = TextObjectFactory.CreateTextObject(spriteRenderer, font, text);
            internalTextObject.StartRendering();
            Text = text;
            ready = true;
            //Allow the text to be changed
            AddArgument("text", text => {
                internalTextObject.TextProperty.Value = text;
            });
        }

        public override void PerformRender()
        {
            if (!ready)
                return;
            IsometricCamera.X = 0.8f;
            //float textWidth = (float)font.MeasureTextWidth(Text, 1);
            //internalTextObject.Scale.Value = Math.Min((Width * 0.003f) / textWidth, Height * 0.003f);          //TODO: This creates tons of objects and spams the GC
            IsometricCamera.Width = Width * 0.003f;
            IsometricCamera.Height = Height * 0.003f;
            spriteRenderer.Render(IsometricCamera);
        }

    }
}

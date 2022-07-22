using CorgEng.Core;
using CorgEng.Core.Dependencies;
using CorgEng.Core.Rendering;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.GenericInterfaces.Font.Fonts;
using CorgEng.GenericInterfaces.Rendering.Renderers.SpriteRendering;
using CorgEng.GenericInterfaces.Rendering.RenderObjects.SpriteRendering;
using CorgEng.GenericInterfaces.Rendering.Text;
using CorgEng.GenericInterfaces.Rendering.Textures;
using CorgEng.GenericInterfaces.UserInterface.Components;
using CorgEng.GenericInterfaces.UserInterface.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Example.Shared.RenderCores
{
    public class ExampleRenderCore : RenderCore
    {

        private Entity renderableEntity;

        [UsingDependency]
        public static ISpriteRendererFactory SpriteRendererFactory;

        public ISpriteRenderer spriteRenderer;

        [UsingDependency]
        public static ISpriteRenderObjectFactory spriteRenderObjectFactory;

        [UsingDependency]
        public static ITextureFactory textureFactory;

        [UsingDependency]
        public static IFontFactory FontFactory;

        [UsingDependency]
        public static ITextObjectFactory TextObjectFactory;

        public override void Initialize()
        {

            spriteRenderer = SpriteRendererFactory.CreateSpriteRenderer(1);

            spriteRenderer?.Initialize();

            IFont font = FontFactory.GetFont("CourierCode");
            ITextObject textObject = TextObjectFactory.CreateTextObject(spriteRenderer, font, "CorgEng.Font");
            textObject.StartRendering();
        }

        public override void PerformRender()
        {
            spriteRenderer?.Render(CorgEngMain.MainCamera);
        }
    }
}

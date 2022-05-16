using CorgEng.Core;
using CorgEng.Core.Dependencies;
using CorgEng.Core.Rendering;
using CorgEng.DependencyInjection;
using CorgEng.DependencyInjection.Injection;
using CorgEng.EntityComponentSystem;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Implementations.Rendering.SpriteRendering;
using CorgEng.EntityComponentSystem.Implementations.Transform;
using CorgEng.Example.Common.Components.Camera;
using CorgEng.Example.Components.PlayerMovement;
using CorgEng.GenericInterfaces.Font.Fonts;
using CorgEng.GenericInterfaces.Networking.Networking;
using CorgEng.GenericInterfaces.Networking.Networking.Server;
using CorgEng.GenericInterfaces.Rendering;
using CorgEng.GenericInterfaces.Rendering.Cameras.Isometric;
using CorgEng.GenericInterfaces.Rendering.Renderers.SpriteRendering;
using CorgEng.GenericInterfaces.Rendering.RenderObjects.SpriteRendering;
using CorgEng.GenericInterfaces.Rendering.Shaders;
using CorgEng.GenericInterfaces.Rendering.Text;
using CorgEng.GenericInterfaces.Rendering.Textures;
using CorgEng.GenericInterfaces.UserInterface.Components;
using CorgEng.GenericInterfaces.UserInterface.Generators;
using CorgEng.UtilityTypes;
using CorgEng.UtilityTypes.Vectors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static OpenGL.Gl;

namespace CorgEng.Example
{
    class Program
    {
        internal class ExampleRenderCore : RenderCore
        {

            private Entity renderableEntity;

            [UsingDependency]
            private static ISpriteRendererFactory SpriteRendererFactory;

            internal ISpriteRenderer spriteRenderer;

            [UsingDependency]
            private static ISpriteRenderObjectFactory spriteRenderObjectFactory;

            [UsingDependency]
            private static ITextureFactory textureFactory;

            //Example user interface
            [UsingDependency]
            private static IUserInterfaceXmlLoader UserInterfaceXmlLoader;

            [UsingDependency]
            private static IFontFactory FontFactory;

            [UsingDependency]
            private static ITextObjectFactory TextObjectFactory;

            private IUserInterfaceComponent rootInterfaceComponent;

            public override void Initialize()
            {

                spriteRenderer = SpriteRendererFactory.CreateSpriteRenderer();

                spriteRenderer?.Initialize();

                //Load a user interface (Yes, I know this shouldn't be in the render core)
                //rootInterfaceComponent = UserInterfaceXmlLoader?.LoadUserInterface("Content/UserInterface/UserInterfaceSimple.xml");
                //rootInterfaceComponent.SetWidth(500, 500);
                //rootInterfaceComponent.Fullscreen = true;

                //Create and setup a renderable thing
                /* SERVER STUFF
                for (int x = 0; x < 39; x++)
                {
                    for (int y = 0; y < 600; y++)
                    {
                        renderableEntity = new Entity();
                        renderableEntity.AddComponent(new SpriteRenderComponent());
                        renderableEntity.AddComponent(new TransformComponent());
                        //renderableEntity.AddComponent(new PlayerMovementComponent());
                        new SetPositionEvent(new Vector<float>(x, y)).Raise(renderableEntity);
                        new SetSpriteEvent("human.ghost").Raise(renderableEntity);
                        new SetSpriteRendererEvent(spriteRenderer).Raise(renderableEntity);
                    }
                }
                */

                IFont font = FontFactory.GetFont("CourierCode");
                ITextObject textObject = TextObjectFactory.CreateTextObject(spriteRenderer, font, "CorgEng.Font");
                textObject.StartRendering();
            }

            public override void PerformRender()
            {
                spriteRenderer?.Render(CorgEngMain.MainCamera);
                //rootInterfaceComponent?.DrawToFramebuffer(FrameBufferUint);
            }
        }

        [UsingDependency]
        private static IIsometricCameraFactory isometricCameraFactory;

        static void Main(string[] args)
        {
            //Load the application config
            CorgEngMain.LoadConfig("CorgEngConfig.xml");
            //Initialize CorgEng
            //This creates the window and loads all
            //modules that are dependencies
            CorgEngMain.Initialize();

            //Camera an isometric camera
            IIsometricCamera camera = isometricCameraFactory.CreateCamera();

            //Create the entity to hold and move the camera
            Entity mainCameraEntity = new Entity();
            mainCameraEntity.AddComponent(new TransformComponent());
            mainCameraEntity.AddComponent(new PlayerMovementComponent());
            mainCameraEntity.AddComponent(new CameraComponent(camera));
            mainCameraEntity.AddComponent(new SpriteRenderComponent());

            //Set the main camera
            CorgEngMain.SetMainCamera(camera);
            //Set the render core
            ExampleRenderCore erc = new ExampleRenderCore();
            CorgEngMain.SetRenderCore(erc);
            new SetSpriteEvent("human.ghost").Raise(mainCameraEntity);
            new SetSpriteRendererEvent(erc.spriteRenderer).Raise(mainCameraEntity);
            //Transfer control of the main thread to the CorgEng
            //rendering thread
            CorgEngMain.TransferToRenderingThread();
            //Shut down the program once it has been closed
            //and clean everything up.
            CorgEngMain.Shutdown();
        }
    }
}

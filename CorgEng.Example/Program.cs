using CorgEng.Core;
using CorgEng.Core.Dependencies;
using CorgEng.Core.Rendering;
using CorgEng.DependencyInjection;
using CorgEng.DependencyInjection.Injection;
using CorgEng.EntityComponentSystem;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.EntityComponentSystem.Implementations.Rendering.SpriteRendering;
using CorgEng.EntityComponentSystem.Implementations.Transform;
using CorgEng.Example.Common.Components.Camera;
using CorgEng.Example.Components.PlayerMovement;
using CorgEng.Example.Shared.RenderCores;
using CorgEng.GenericInterfaces.Font.Fonts;
using CorgEng.GenericInterfaces.Networking.Networking;
using CorgEng.GenericInterfaces.Networking.Networking.Client;
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

        [UsingDependency]
        private static IIsometricCameraFactory isometricCameraFactory;

        [UsingDependency]
        private static INetworkingClient NetworkingClient;

        static void Main(string[] args)
        {
            //Load the application config
            CorgEngMain.LoadConfig("CorgEngConfig.xml");
            //Initialize CorgEng
            //This creates the window and loads all
            //modules that are dependencies
            CorgEngMain.Initialize();

            //Set the render core
            ExampleRenderCore erc = new ExampleRenderCore();
            CorgEngMain.SetRenderCore(erc);

            //Connect to our server
            NetworkingClient.AttemptConnection("127.0.0.1", 5000);

            //Camera an isometric camera
            IIsometricCamera camera = isometricCameraFactory.CreateCamera();

            //Create the entity to hold and move the camera
            Entity mainCameraEntity = new Entity();
            mainCameraEntity.AddComponent(new TransformComponent());
            mainCameraEntity.AddComponent(new PlayerMovementComponent());
            mainCameraEntity.AddComponent(new CameraComponent(camera));
            mainCameraEntity.AddComponent(new SpriteRenderComponent());
            new SetSpriteEvent("human.ghost").Raise(mainCameraEntity);
            new SetSpriteRendererEvent(erc.spriteRenderer).Raise(mainCameraEntity);

            //Set the main camera
            CorgEngMain.SetMainCamera(camera);
            //Transfer control of the main thread to the CorgEng
            //rendering thread
            CorgEngMain.TransferToRenderingThread();
            //Shut down the program once it has been closed
            //and clean everything up.
            CorgEngMain.Shutdown();
        }
    }
}

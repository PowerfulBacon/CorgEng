using CorgEng.Core;
using CorgEng.Core.Dependencies;
using CorgEng.Example.Modules.CameraScroll;
using CorgEng.Example.Shared.RenderCores;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Networking.Networking.Client;
using CorgEng.GenericInterfaces.Rendering.Cameras.Isometric;
using System.Threading;


namespace CorgEng.Example
{
    class Program
    {

        [UsingDependency]
        private static IIsometricCameraFactory isometricCameraFactory = null!;

        [UsingDependency]
        private static IWorldFactory WorldFactory = null!;

        static void Main(string[] args)
        {
            //Load the application config
            CorgEngMain.LoadConfig("CorgEngConfig.xml");
            //Initialize CorgEng
            //This creates the window and loads all
            //modules that are dependencies
            CorgEngMain.Initialize();

            // Create the world
            IWorld world = WorldFactory.CreateWorld();

            //Set the render core

            // Prevent failures
            //Thread.Sleep(5000);

            //Camera an isometric camera
            IIsometricCamera camera = isometricCameraFactory.CreateCamera();
            CameraScrollSystem.IsometricCamera = camera;

            ExampleRenderCore erc = new ExampleRenderCore(world);
            CorgEngMain.SetRenderCore(erc);

            //Connect to our server
            world.ClientInstance.AttemptConnection("127.0.0.1", 5000);

            //Create the entity to hold and move the camera
            /*Entity mainCameraEntity = new Entity();
            mainCameraEntity.AddComponent(new TransformComponent());
            mainCameraEntity.AddComponent(new PlayerMovementComponent());
            mainCameraEntity.AddComponent(new CameraComponent(camera));
            mainCameraEntity.AddComponent(new SpriteRenderComponent());*/
            /*
            new SetSpriteEvent("human.ghost").Raise(mainCameraEntity);
            new SetSpriteRendererEvent(erc.spriteRenderer).Raise(mainCameraEntity);
            */
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

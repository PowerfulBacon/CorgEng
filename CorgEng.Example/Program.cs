using CorgEng.Core;
using CorgEng.Core.Dependencies;
using CorgEng.Example.Modules.CameraScroll;
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
            //Initialize CorgEng
            //This creates the window and loads all
            //modules that are dependencies
            CorgEngMain.Initialize("CorgEngConfig.xml");

            //Camera an isometric camera
            IIsometricCamera camera = isometricCameraFactory.CreateCamera();
            CameraScrollSystem.IsometricCamera = camera;

            //Connect to our server
            CorgEngMain.World.ClientInstance.AttemptConnection("127.0.0.1", 5000);

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

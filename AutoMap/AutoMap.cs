using AutoMap.Rendering;
using CorgEng.Core;
using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Implementations.Transform;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Rendering.Cameras.Isometric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMap
{
    class AutoMap
    {

        [UsingDependency]
        private static IIsometricCameraFactory isometricCameraFactory;

        [UsingDependency]
        private static IEntityFactory EntityFactory;

        [STAThread]
        public static void Main(string[] args)
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
            EntityFactory.CreateEmptyEntity((mainCameraEntity) => {
                mainCameraEntity.AddComponent(new TransformComponent());
                //mainCameraEntity.AddComponent(new PlayerMovementComponent());
                //mainCameraEntity.AddComponent(new CameraComponent(camera));
            });

            //Set the main camera
            CorgEngMain.SetMainCamera(camera);
            //Set the render core
            AutoMapRenderCore renderCore = new AutoMapRenderCore();
            CorgEngMain.SetRenderCore(renderCore);
            //Transfer control of the main thread to the CorgEng
            //rendering thread
            CorgEngMain.TransferToRenderingThread();
            //Shut down the program once it has been closed
            //and clean everything up.
            CorgEngMain.Shutdown();
            //Ask for a line of code
#if DEBUG
            Console.WriteLine("Program finished execution, press any key to continue...");
            Console.ReadKey();
#endif
        }

    }
}

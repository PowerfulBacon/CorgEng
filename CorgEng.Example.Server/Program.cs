#define DEBUG_RENDERING

using CorgEng.Core;
using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.EntityComponentSystem.Implementations.Deletion;
using CorgEng.EntityComponentSystem.Implementations.Rendering.SpriteRendering;
using CorgEng.EntityComponentSystem.Implementations.Transform;
using CorgEng.Example.Components.PlayerMovement;
using CorgEng.Example.Shared.RenderCores;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Networking.Networking.Server;
using CorgEng.GenericInterfaces.Networking.PrototypeManager;
using CorgEng.GenericInterfaces.Rendering.Cameras.Isometric;
using CorgEng.GenericInterfaces.Rendering.Icons;
using CorgEng.InputHandling.Events;
using CorgEng.Networking.Components;
using CorgEng.UtilityTypes.Vectors;
using GLFW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CorgEng.Example.Server
{
    class Program
    {

        [UsingDependency]
        private static ILogger Logger;

        [UsingDependency]
        private static INetworkingServer NetworkingServer;

        [UsingDependency]
        private static IPrototypeManager PrototypeManager;

        [UsingDependency]
        private static IEntityFactory EntityFactory;

#if DEBUG_RENDERING
        [UsingDependency]
        private static IIsometricCameraFactory IsometricCameraFactory;
#endif

        [UsingDependency]
        private static IIconFactory IconFactory;

        static void Main(string[] args)
        {
            //Load the application config
            CorgEngMain.LoadConfig("CorgEngConfig.xml");
            CorgEngMain.WindowName = "CorgEngApplication Server";
            //Initialize CorgEng in headless mode
#if !DEBUG_RENDERING
            CorgEngMain.Initialize(true);
#else
            CorgEngMain.Initialize();

            ExampleRenderCore erc = new ExampleRenderCore();
            CorgEngMain.SetRenderCore(erc);
            IIsometricCamera camera = IsometricCameraFactory.CreateCamera();
            CorgEngMain.SetMainCamera(camera);
#endif

            //Create a testing entity
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    EntityFactory.CreateEmptyEntity(testingEntity => {
                        //Add components
                        testingEntity.AddComponent(new NetworkTransformComponent());
                        testingEntity.AddComponent(new SpriteRenderComponent());
                        //Update the entity
                        new SetPositionEvent(new Vector<float>(x, y)).Raise(testingEntity);
                        new SetSpriteEvent(IconFactory.CreateIcon("human.ghost", 5)).Raise(testingEntity);
                        new SetSpriteRendererEvent(1).Raise(testingEntity);
                    });
                }
            }
            //Set the default player prototype
            SetPlayerPrototype();
            //Start networking server
            NetworkingServer.StartHosting(5000);

#if DEBUG_RENDERING
            //Transfer control of the main thread to the CorgEng
            //rendering thread
            CorgEngMain.TransferToRenderingThread();
            //Shut down the program once it has been closed
            //and clean everything up.
            CorgEngMain.Shutdown();
#else
            while (true)
            {
                Thread.Sleep(100);
            }
#endif
        }

        private static void SetPlayerPrototype()
        {
            EntityFactory.CreateEmptyEntity(playerPrototype => {
                playerPrototype.AddComponent(new ClientComponent());
                playerPrototype.AddComponent(new NetworkTransformComponent());
                playerPrototype.AddComponent(new SpriteRenderComponent() { Sprite = IconFactory.CreateIcon("human.ghost", 5), SpriteRendererIdentifier = 1 });
                playerPrototype.AddComponent(new PlayerMovementComponent());
                playerPrototype.AddComponent(new DeleteableComponent());
                IPrototype prototype = PrototypeManager.GetPrototype(playerPrototype);
                NetworkingServer.SetClientPrototype(prototype);
                new DeleteEntityEvent().Raise(playerPrototype);
            });
        }

    }
}

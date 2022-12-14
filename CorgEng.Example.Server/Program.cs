#define DEBUG_RENDERING

using CorgEng.Core;
using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.EntityComponentSystem.Implementations.Deletion;
using CorgEng.EntityComponentSystem.Implementations.Rendering.SpriteRendering;
using CorgEng.EntityComponentSystem.Implementations.Transform;
using CorgEng.EntityComponentSystem.Systems;
using CorgEng.Example.Common.Components.Camera;
using CorgEng.Example.Components.PlayerMovement;
using CorgEng.Example.Shared.Components.Gravity;
using CorgEng.Example.Shared.Components.SandFactory;
using CorgEng.Example.Shared.RenderCores;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Networking.Config;
using CorgEng.GenericInterfaces.Networking.Networking.Server;
using CorgEng.GenericInterfaces.Networking.PrototypeManager;
using CorgEng.GenericInterfaces.Rendering.Cameras.Isometric;
using CorgEng.GenericInterfaces.Rendering.Icons;
using CorgEng.InputHandling.Events;
using CorgEng.Networking.Components;
using CorgEng.Pathfinding.Components;
using CorgEng.UtilityTypes.Vectors;
using GLFW;
using System;
using System.Collections.Generic;
using System.IO;
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


        [UsingDependency]
        private static INetworkConfig NetworkConfig;

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
            EntityFactory.CreateEmptyEntity(entity => {
                IIsometricCamera camera = IsometricCameraFactory.CreateCamera();
                camera.Width = 30;
                camera.Height = 30;
                entity.AddComponent(new TransformComponent());
                entity.AddComponent(new PlayerMovementComponent());
                entity.AddComponent(new CameraComponent(camera));
                CorgEngMain.SetMainCamera(camera);
                new SetPositionEvent(new Vector<float>(500, -15)).Raise(entity);
            });

            //Debug
            NetworkConfig.ProcessClientSystems = true;
#endif

            BuildWorld();

            //Create a testing entity
            EntityFactory.CreateEmptyEntity(testingEntity => {
                //Add components
                testingEntity.AddComponent(new NetworkTransformComponent());
                testingEntity.AddComponent(new SpriteRenderComponent());
                testingEntity.AddComponent(new SandFactoryComponent());
                //Update the entity
                new SetPositionEvent(new Vector<float>(500, 0)).Raise(testingEntity);
                new SetSpriteEvent(IconFactory.CreateIcon("sand", 5)).Raise(testingEntity);
                new SetSpriteRendererEvent(1).Raise(testingEntity);
            });

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

        /// <summary>
        /// Build the world
        /// </summary>
        private static void BuildWorld()
        {
            //Parse input
            File.ReadAllLines("Content/world.txt")
                .Select(x => x.Split(" -> ")
                    .Select(x =>
                    {
                        string[] splitInput = x.Split(",");
                        return (int.Parse(splitInput[0]), int.Parse(splitInput[1]));
                    }))
                .ToList()
                .ForEach(X => {
                    (int, int)? previous = null;
                    X.ToList().ForEach(x =>
                    {
                        if (previous == null)
                        {
                            previous = x;
                            return;
                        }
                        //Draw a line
                        int start_x = previous.Value.Item1;
                        int start_y = previous.Value.Item2;
                        int end_x = x.Item1;
                        int end_y = x.Item2;
                        for (int xv = Math.Min(start_x, end_x); xv <= Math.Max(start_x, end_x); xv++)
                        {
                            for (int yv = Math.Min(start_y, end_y); yv <= Math.Max(start_y, end_y); yv++)
                            {
                                EntityFactory.CreateEmptyEntity(testingEntity => {
                                    //Add components
                                    testingEntity.AddComponent(new NetworkTransformComponent());
                                    testingEntity.AddComponent(new SpriteRenderComponent());
                                    testingEntity.AddComponent(new SolidComponent());
                                    //Update the entity
                                    new SetPositionEvent(new Vector<float>(xv, -yv)).Raise(testingEntity);
                                    new SetSpriteEvent(IconFactory.CreateIcon("rock", 5)).Raise(testingEntity);
                                    new SetSpriteRendererEvent(1).Raise(testingEntity);
                                });
                            }
                        }
                        previous = x;
                    });
                });
        }

        private static void SetPlayerPrototype()
        {
            EntityFactory.CreateEmptyEntity(playerPrototype => {
                playerPrototype.AddComponent(new ClientComponent());
                playerPrototype.AddComponent(new NetworkTransformComponent());
                playerPrototype.AddComponent(new SpriteRenderComponent() { Sprite = IconFactory.CreateIcon("human.ghost", 5), SpriteRendererIdentifier = 1 });
                playerPrototype.AddComponent(new PlayerMovementComponent());
                IPrototype prototype = PrototypeManager.GetPrototype(playerPrototype);
                NetworkingServer.SetClientPrototype(prototype);
                new DeleteEntityEvent().Raise(playerPrototype);
            });
        }

    }
}

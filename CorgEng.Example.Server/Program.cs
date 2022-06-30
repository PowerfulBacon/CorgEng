using CorgEng.Core;
using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.EntityComponentSystem.Implementations.Rendering.SpriteRendering;
using CorgEng.EntityComponentSystem.Implementations.Transform;
using CorgEng.Example.Components.PlayerMovement;
using CorgEng.Example.Shared.RenderCores;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Networking.Networking.Server;
using CorgEng.InputHandling.Events;
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

        static void Main(string[] args)
        {
            //Load the application config
            CorgEngMain.LoadConfig("CorgEngConfig.xml");
            //Initialize CorgEng in headless mode
            CorgEngMain.Initialize(true);

            //Create a testing entity
            for (int x = 1; x <= 1; x++)
            {
                for (int y = 1; y <= 1; y++)
                {
                    IEntity testingEntity = new Entity();
                    //Add components
                    testingEntity.AddComponent(new TransformComponent());
                    testingEntity.AddComponent(new PlayerMovementComponent());
                    testingEntity.AddComponent(new SpriteRenderComponent());
                    //Update the entity
                    new SetPositionEvent(new Vector<float>(x, y)).Raise(testingEntity);
                    new SetSpriteEvent("human.ghost").Raise(testingEntity);
                    new SetSpriteRendererEvent(1).Raise(testingEntity);
                }
            }
            //Start networking server
            NetworkingServer.StartHosting(5000);

            while (true)
            {
                Thread.Sleep(100);
            }
        }
    }
}

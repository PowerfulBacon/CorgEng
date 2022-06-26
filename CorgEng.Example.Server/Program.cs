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

        //Networked render core
        //This works because the render core contains a reference to a sprite renderer
        private static ExampleRenderCore networkedRenderCore = new ExampleRenderCore();

        static void Main(string[] args)
        {
            //Load the application config
            CorgEngMain.LoadConfig("CorgEngConfig.xml");
            //Initialize CorgEng in headless mode
            CorgEngMain.Initialize(true);

            //Start networking server
            NetworkingServer.StartHosting(5000);

            //Create a testing entity
            IEntity testingEntity = new Entity();
            //Add components
            testingEntity.AddComponent(new TransformComponent());
            testingEntity.AddComponent(new PlayerMovementComponent());
            testingEntity.AddComponent(new SpriteRenderComponent());
            //Update the entity
            new SetPositionEvent(new Vector<float>(0, 1)).Raise(testingEntity);
            new SetSpriteEvent("human.ghost").Raise(testingEntity);
            new SetSpriteRendererEvent(networkedRenderCore.spriteRenderer).Raise(testingEntity);

            while (true)
            {
                Thread.Sleep(100);
            }
        }
    }
}

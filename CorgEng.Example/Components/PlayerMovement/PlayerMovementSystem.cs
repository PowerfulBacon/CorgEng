using CorgEng.Core;
using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Components;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.EntityComponentSystem.Implementations.Transform;
using CorgEng.EntityComponentSystem.Systems;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.InputHandling.Events;
using CorgEng.UtilityTypes.Vectors;
using GLFW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Example.Components.PlayerMovement
{
    public class PlayerMovementSystem : EntitySystem
    {

        private static List<Entity> playerEntities = new List<Entity>();

        public override void SystemSetup()
        {
            RegisterLocalEvent<PlayerMovementComponent, ComponentAddedEvent>(OnComponentAdded);
            RegisterGlobalEvent<KeyHeldEvent>(OnKeyHeld);
        }

        public void OnComponentAdded(Entity entity, PlayerMovementComponent playerMovementComponent, ComponentAddedEvent componentAddEvent)
        {
            if (componentAddEvent.Component != playerMovementComponent)
                return;
            playerEntities.Add(entity);
        }

        public void OnKeyHeld(KeyHeldEvent keyHeldEvent)
        {
            switch (keyHeldEvent.Key)
            {
                case Keys.W:
                    playerEntities.ForEach((Entity entity) => {
                        new TranslateEvent(new Vector<float>(0, (float)CorgEngMain.DeltaTime)).Raise(entity);
                    });
                    break;
                case Keys.S:
                    playerEntities.ForEach((Entity entity) => {
                        new TranslateEvent(new Vector<float>(0, -(float)CorgEngMain.DeltaTime)).Raise(entity);
                    });
                    break;
                case Keys.D:
                    playerEntities.ForEach((Entity entity) => {
                        new TranslateEvent(new Vector<float>((float)CorgEngMain.DeltaTime, 0)).Raise(entity);
                    });
                    break;
                case Keys.A:
                    playerEntities.ForEach((Entity entity) => {
                        new TranslateEvent(new Vector<float>(-(float)CorgEngMain.DeltaTime, 0)).Raise(entity);
                    });
                    break;
            }
        }

    }
}

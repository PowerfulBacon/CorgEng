﻿using CorgEng.Core;
using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Components;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.EntityComponentSystem.Implementations.Transform;
using CorgEng.EntityComponentSystem.Systems;
using CorgEng.GenericInterfaces.EntityComponentSystem;
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

        [UsingDependency]
        private static ILogger Logger;

        private static List<IEntity> playerEntities = new List<IEntity>();

        //Logic executed on the server that moves players when requested.
        public override EntitySystemFlags SystemFlags { get; } = EntitySystemFlags.HOST_SYSTEM;

        public override void SystemSetup(IWorld world)
        {
            RegisterLocalEvent<PlayerMovementComponent, ComponentAddedEvent>(OnComponentAdded);
            RegisterLocalEvent<PlayerMovementComponent, ComponentRemovedEvent>(OnComponentRemoved);
            RegisterGlobalEvent<KeyPressEvent>(OnKeyPress);
        }

        public void OnComponentRemoved(IEntity entity, PlayerMovementComponent playerMovementComponent, ComponentRemovedEvent componentRemovedEvent)
        {
            if (componentRemovedEvent.Component != playerMovementComponent)
                return;
            playerEntities.Remove(entity);
        }

        public void OnComponentAdded(IEntity entity, PlayerMovementComponent playerMovementComponent, ComponentAddedEvent componentAddEvent)
        {
            if (componentAddEvent.Component != playerMovementComponent)
                return;
            playerEntities.Add(entity);
        }

        public void OnKeyPress(KeyPressEvent keyPressEvent)
        {
            switch (keyPressEvent.Key)
            {
                case Keys.W:
                    playerEntities.ForEach((IEntity entity) => {
                        new TranslateEvent(new Vector<float>(0, 1)).Raise(entity);
                    });
                    break;
                case Keys.S:
                    playerEntities.ForEach((IEntity entity) => {
                        new TranslateEvent(new Vector<float>(0, -1)).Raise(entity);
                    });
                    break;
                case Keys.D:
                    playerEntities.ForEach((IEntity entity) => {
                        new TranslateEvent(new Vector<float>(1, 0)).Raise(entity);
                    });
                    break;
                case Keys.A:
                    playerEntities.ForEach((IEntity entity) => {
                        new TranslateEvent(new Vector<float>(-1, 0)).Raise(entity);
                    });
                    break;
                case Keys.K:
                    playerEntities.ForEach((IEntity entity) => {
                        foreach(IComponent component in entity.Components)
                        {
                            if (component is PlayerMovementComponent)
                            {
                                entity.RemoveComponent(component, true);
                                return;
                            }
                        }
                    });
                    break;
            }
        }

    }
}

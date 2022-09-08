using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.EntityComponentSystem.Implementations.Transform;
using CorgEng.EntityComponentSystem.Systems;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.World;
using CorgEng.World.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.World.EntitySystems
{
    internal class WorldSystem : EntitySystem
    {

        [UsingDependency]
        private static IWorld WorldAccess;

        public override EntitySystemFlags SystemFlags { get; } = EntitySystemFlags.HOST_SYSTEM;

        public override void SystemSetup()
        {
            RegisterLocalEvent<TrackComponent, ComponentAddedEvent>(OnEntityCreated);
            RegisterLocalEvent<TrackComponent, MoveEvent>(OnEntityMoved);
            RegisterLocalEvent<TrackComponent, ComponentRemovedEvent>(OnComponentRemoved);
        }

        /// <summary>
        /// When the transform component is added to an entity, it needs
        /// to begin tracking in the world system.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="transformComponent"></param>
        /// <param name="componentAddedEvent"></param>
        private void OnEntityCreated(IEntity entity, TrackComponent trackComponent, ComponentAddedEvent componentAddedEvent)
        {
            if (componentAddedEvent.Component != trackComponent)
                return;
            //Add the entity to the world
            WorldAccess.AddEntity(trackComponent.Key, trackComponent, trackComponent.Transform.Position.X, trackComponent.Transform.Position.Y, 0);
        }

        /// <summary>
        /// When the entity moves it needs to be updated in the world system.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="transformComponent"></param>
        /// <param name="moveEvent"></param>
        private void OnEntityMoved(IEntity entity, TrackComponent trackComponent, MoveEvent moveEvent)
        {
            WorldAccess.RemoveEntity(trackComponent.Key, trackComponent, moveEvent.OldPosition.X, moveEvent.OldPosition.Y, 0);
            WorldAccess.AddEntity(trackComponent.Key, trackComponent, moveEvent.NewPosition.X, moveEvent.NewPosition.Y, 0);
        }

        /// <summary>
        /// Stop tracking a component when the transform is removed
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="transformComponent"></param>
        /// <param name="componentRemovedEvent"></param>
        private void OnComponentRemoved(IEntity entity, TrackComponent trackComponent, ComponentRemovedEvent componentRemovedEvent)
        {
            if (componentRemovedEvent.Component != trackComponent)
                return;
            WorldAccess.RemoveEntity(trackComponent.Key, trackComponent, trackComponent.Transform.Position.X, trackComponent.Transform.Position.Y, 0);
        }

    }
}

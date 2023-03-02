using CorgEng.Contents.Components;
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
    public class WorldSystem : EntitySystem
    {

        [UsingDependency]
        private static IWorld WorldAccess = null!;

        public override EntitySystemFlags SystemFlags { get; } = EntitySystemFlags.HOST_SYSTEM;

        public override void SystemSetup()
        {
            RegisterLocalEvent<TrackComponent, ComponentAddedEvent>(OnEntityCreated);
            RegisterLocalEvent<TrackComponent, ContentsChangedEvent>(OnEntityLocationChanged);
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
            //Check to ensure that we are not inside something
            if (trackComponent.Parent.HasComponent<ContainedComponent>())
                return;
            //Add the entity to the world
            WorldAccess.AddEntity(trackComponent.Key, trackComponent, trackComponent.Transform.Position.Value.X, trackComponent.Transform.Position.Value.Y, 0);
            trackComponent.isTracking = true;
        }

        /// <summary>
        /// When the entity has its location changed, it needs to be freed
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="trackComponent"></param>
        /// <param name="contentsChangedEvent"></param>
        private void OnEntityLocationChanged(IEntity entity, TrackComponent trackComponent, ContentsChangedEvent contentsChangedEvent)
        {
            if (trackComponent.isTracking && contentsChangedEvent.NewHolder != null)
            {
                //Stop tracking
                trackComponent.isTracking = false;
                WorldAccess.RemoveEntity(trackComponent.Key, trackComponent, trackComponent.ContentsLocation.X, trackComponent.ContentsLocation.Y, 0);
            }
            else if (!trackComponent.isTracking && contentsChangedEvent.NewHolder == null)
            {
                //Start tracking
                trackComponent.isTracking = true;
                WorldAccess.AddEntity(trackComponent.Key, trackComponent, trackComponent.Transform.Position.Value.X, trackComponent.Transform.Position.Value.Y, 0);
            }
        }

        /// <summary>
        /// When the entity moves it needs to be updated in the world system.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="transformComponent"></param>
        /// <param name="moveEvent"></param>
        private void OnEntityMoved(IEntity entity, TrackComponent trackComponent, MoveEvent moveEvent)
        {
            //Not currently tracking, somehow this was called while we were inside somethings contents.
            if (!trackComponent.isTracking)
                return;
            WorldAccess.RemoveEntity(trackComponent.Key, trackComponent, trackComponent.ContentsLocation.X, trackComponent.ContentsLocation.Y, 0);
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
            //We were never being tracked due to being inside another entity.
            if (!trackComponent.isTracking)
                return;
            WorldAccess.RemoveEntity(trackComponent.Key, trackComponent, trackComponent.ContentsLocation.X, trackComponent.ContentsLocation.Y, 0);
            trackComponent.isTracking = false;
        }

    }
}

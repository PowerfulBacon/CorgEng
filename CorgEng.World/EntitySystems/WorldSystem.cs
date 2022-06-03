using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.EntityComponentSystem.Implementations.Transform;
using CorgEng.EntityComponentSystem.Systems;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.World.EntitySystems
{
    internal class WorldSystem : EntitySystem
    {

        public override EntitySystemFlags SystemFlags { get; } = EntitySystemFlags.HOST_SYSTEM;

        public override void SystemSetup()
        {
            RegisterLocalEvent<TransformComponent, ComponentAddedEvent>(OnEntityCreated);
            RegisterLocalEvent<TransformComponent, MoveEvent>(OnEntityMoved);
        }

        /// <summary>
        /// When the transform component is added to an entity, it needs
        /// to begin tracking in the world system.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="transformComponent"></param>
        /// <param name="componentAddedEvent"></param>
        private void OnEntityCreated(IEntity entity, TransformComponent transformComponent, ComponentAddedEvent componentAddedEvent)
        {
            
        }

        /// <summary>
        /// When the entity moves it needs to be updated in the world system.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="transformComponent"></param>
        /// <param name="moveEvent"></param>
        private void OnEntityMoved(IEntity entity, TransformComponent transformComponent, MoveEvent moveEvent)
        {

        }

    }
}

using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.EntityComponentSystem.Implementations.Transform;
using CorgEng.EntityComponentSystem.Systems;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.Pathfinding.Components;
using CorgEng.Pathfinding.Pathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Pathfinding.Systems
{
    internal class SolidSystem : EntitySystem
    {

        /// <summary>
        /// The world layers
        /// </summary>
        private static Dictionary<int, WorldGrid> WorldLayers = new Dictionary<int, WorldGrid>();

        public override EntitySystemFlags SystemFlags => EntitySystemFlags.HOST_SYSTEM;

        public override void SystemSetup()
        {
            RegisterLocalEvent<SolidComponent, ComponentAddedEvent>(OnComponentAdded);
            RegisterLocalEvent<SolidComponent, ComponentRemovedEvent>(OnComponentRemoved);
            RegisterLocalEvent<SolidComponent, MoveEvent>(OnEntityMove);
        }

        private void OnComponentAdded(IEntity entity, SolidComponent solidComponent, ComponentAddedEvent componentAddedEvent)
        {
            if (componentAddedEvent.Component != solidComponent)
                return;
            //Locate the attached transform component
            TransformComponent attachedTransformComponent = entity.GetComponent<TransformComponent>();
            if (attachedTransformComponent == null)
                return;
            //Add to the world layers list (TODO: Add support for z-levels)
            if (!WorldLayers.ContainsKey(0))
            {
                WorldLayers.Add(0, new WorldGrid());
            }
            //Get the position
            int positionX = (int)Math.Round(attachedTransformComponent.Position.X);
            int positionY = (int)Math.Round(attachedTransformComponent.Position.Y);
            WorldLayers[0].AddElement(positionX, positionY);
        }

        private void OnComponentRemoved(IEntity entity, SolidComponent solidComponent, ComponentRemovedEvent componentRemoveEvent)
        {
            if (componentRemoveEvent.Component != solidComponent)
                return;
            //Locate the attached transform component
            TransformComponent attachedTransformComponent = entity.GetComponent<TransformComponent>();
            if (attachedTransformComponent == null)
                return;
            //Add to the world layers list (TODO: Add support for z-levels)
            if (!WorldLayers.ContainsKey(0))
            {
                return;
            }
            //Get the position
            int positionX = (int)Math.Round(attachedTransformComponent.Position.X);
            int positionY = (int)Math.Round(attachedTransformComponent.Position.Y);
            WorldLayers[0].RemoveElement(positionX, positionY);
        }

        private void OnEntityMove(IEntity entity, SolidComponent solidComponent, MoveEvent moveEvent)
        {
            int oldPositionX = (int)Math.Round(moveEvent.OldPosition.X);
            int oldPositionY = (int)Math.Round(moveEvent.OldPosition.Y);
            int newPositionX = (int)Math.Round(moveEvent.NewPosition.X);
            int newPositionY = (int)Math.Round(moveEvent.NewPosition.Y);
            //Remove from old position :(
            WorldLayers[0].RemoveElement(oldPositionX, oldPositionY);
            //Add to new position :)
            WorldLayers[0].AddElement(newPositionX, newPositionY);
        }

    }
}

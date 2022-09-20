using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.EntityComponentSystem.Implementations.Rendering.SpriteRendering;
using CorgEng.EntityComponentSystem.Implementations.Transform;
using CorgEng.EntityComponentSystem.Systems;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Rendering.Icons;
using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.GenericInterfaces.World;
using CorgEng.IconSmoothing.Components;
using CorgEng.IconSmoothing.Events;
using CorgEng.UtilityTypes.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.IconSmoothing.Systems
{
    internal class SmoothIconSystem : EntitySystem
    {

        [UsingDependency]
        private static IWorld WorldAccess;

        public override EntitySystemFlags SystemFlags => EntitySystemFlags.HOST_SYSTEM;

        public override void SystemSetup()
        {
            RegisterLocalEvent<SmoothIconComponent, ComponentAddedEvent>(OnEntityCreated);
            RegisterLocalEvent<SmoothIconComponent, MoveEvent>(OnEntityMoved);
            RegisterLocalEvent<SmoothIconComponent, ComponentRemovedEvent>(OnComponentRemoved);
            RegisterLocalEvent<SmoothIconComponent, CalculateSmoothEvent>(SmoothEntity);
        }

        private void SmoothEntity(IEntity entity, SmoothIconComponent smoothIconComponent, CalculateSmoothEvent calculateSmoothEvent)
        {
            IVector<float> position = smoothIconComponent.TransformComponent.Position;
            IVector<int> above = new Vector<int>((int)position.X, (int)position.Y + 1);
            IVector<int> below = new Vector<int>((int)position.X, (int)position.Y - 1);
            IVector<int> right = new Vector<int>((int)position.X + 1, (int)position.Y);
            IVector<int> left = new Vector<int>((int)position.X - 1, (int)position.Y);
            DirectionalState directionalState = DirectionalState.NONE;
            if (WorldAccess.GetContentsAt(smoothIconComponent.Key, above.X, above.Y, 0)?.Count > 0)
            {
                directionalState |= DirectionalState.NORTH;
            }
            if (WorldAccess.GetContentsAt(smoothIconComponent.Key, below.X, below.Y, 0)?.Count > 0)
            {
                directionalState |= DirectionalState.SOUTH;
            }
            if (WorldAccess.GetContentsAt(smoothIconComponent.Key, right.X, right.Y, 0)?.Count > 0)
            {
                directionalState |= DirectionalState.EAST;
            }
            if (WorldAccess.GetContentsAt(smoothIconComponent.Key, left.X, left.Y, 0)?.Count > 0)
            {
                directionalState |= DirectionalState.WEST;
            }
            new SetDirectionEvent(directionalState).Raise(entity);
        }

        /// <summary>
        /// When the transform component is added to an entity, it needs
        /// to begin tracking in the world system.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="transformComponent"></param>
        /// <param name="componentAddedEvent"></param>
        private void OnEntityCreated(IEntity entity, SmoothIconComponent trackComponent, ComponentAddedEvent componentAddedEvent)
        {
            if (componentAddedEvent.Component != trackComponent)
                return;
            SmoothAround(trackComponent);
        }

        /// <summary>
        /// When the entity moves it needs to be updated in the world system.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="transformComponent"></param>
        /// <param name="moveEvent"></param>
        private void OnEntityMoved(IEntity entity, SmoothIconComponent trackComponent, MoveEvent moveEvent)
        {
            SmoothAround(trackComponent);
        }

        /// <summary>
        /// Stop tracking a component when the transform is removed
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="transformComponent"></param>
        /// <param name="componentRemovedEvent"></param>
        private void OnComponentRemoved(IEntity entity, SmoothIconComponent trackComponent, ComponentRemovedEvent componentRemovedEvent)
        {
            if (componentRemovedEvent.Component != trackComponent)
                return;
            SmoothAround(trackComponent);
        }

        private void SmoothAround(SmoothIconComponent smoothIconComponent)
        {
            IVector<float> position = smoothIconComponent.TransformComponent.Position;
            IVector<int> above = new Vector<int>((int)position.X, (int)position.Y + 1);
            IVector<int> below = new Vector<int>((int)position.X, (int)position.Y - 1);
            IVector<int> right = new Vector<int>((int)position.X + 1, (int)position.Y);
            IVector<int> left = new Vector<int>((int)position.X - 1, (int)position.Y);
            IEnumerable<IWorldTrackComponent> a = new IWorldTrackComponent[0];
            foreach (IWorldTrackComponent smoothIcon in WorldAccess.GetContentsAt(smoothIconComponent.Key, above.X, above.Y, 0)?.GetContents() ?? a)
            {
                new CalculateSmoothEvent().Raise(smoothIcon.Parent);
            }
            foreach (IWorldTrackComponent smoothIcon in WorldAccess.GetContentsAt(smoothIconComponent.Key, below.X, below.Y, 0)?.GetContents() ?? a)
            {
                new CalculateSmoothEvent().Raise(smoothIcon.Parent);
            }
            foreach (IWorldTrackComponent smoothIcon in WorldAccess.GetContentsAt(smoothIconComponent.Key, right.X, right.Y, 0)?.GetContents() ?? a)
            {
                new CalculateSmoothEvent().Raise(smoothIcon.Parent);
            }
            foreach (IWorldTrackComponent smoothIcon in WorldAccess.GetContentsAt(smoothIconComponent.Key, left.X, left.Y, 0)?.GetContents() ?? a)
            {
                new CalculateSmoothEvent().Raise(smoothIcon.Parent);
            }
            new CalculateSmoothEvent().Raise(smoothIconComponent.Parent);
        }

    }
}

using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.EntityComponentSystem.Implementations.Transform;
using CorgEng.EntityComponentSystem.Systems;
using CorgEng.InputHandling.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Example.Common.Components.Camera
{
    public class CameraSystem : EntitySystem
    {

        private static List<Entity> playerEntities = new List<Entity>();

        public override void SystemSetup()
        {
            RegisterLocalEvent<CameraComponent, MoveEvent>(OnCameraMoved);
            RegisterLocalEvent<CameraComponent, ComponentAddedEvent>(OnComponentAdded);
            RegisterLocalEvent<CameraComponent, ComponentRemovedEvent>(OnComponentRemoved);
            RegisterGlobalEvent<MouseScrollEvent>(OnMouseScroll);
            RegisterLocalEvent<CameraComponent, MouseScrollEvent>(OnMouseScrolled);
        }

        private void OnCameraMoved(Entity entity, CameraComponent cameraComponent, MoveEvent moveEvent)
        {
            cameraComponent.Camera.X = moveEvent.NewPosition.X;
            cameraComponent.Camera.Y = moveEvent.NewPosition.Y;
        }

        public void OnComponentRemoved(Entity entity, CameraComponent cameraComponent, ComponentRemovedEvent componentRemovedEvent)
        {
            if (componentRemovedEvent.Component != cameraComponent)
                return;
            playerEntities.Remove(entity);
        }

        public void OnComponentAdded(Entity entity, CameraComponent cameraComponent, ComponentAddedEvent componentAddEvent)
        {
            if (componentAddEvent.Component != cameraComponent)
                return;
            playerEntities.Add(entity);
        }

        //cheese
        public void OnMouseScroll(MouseScrollEvent scrollEvent)
        {
            foreach (Entity entity in playerEntities)
            {
                scrollEvent.Raise(entity);
            }
        }

        public void OnMouseScrolled(Entity entity, CameraComponent cameraComponent, MouseScrollEvent scrollEvent)
        {
            cameraComponent.Camera.Width -= (float)scrollEvent.ScrollDelta;
            cameraComponent.Camera.Height -= (float)scrollEvent.ScrollDelta;
        }

    }
}

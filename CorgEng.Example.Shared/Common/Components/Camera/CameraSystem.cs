using CorgEng.Core;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.EntityComponentSystem.Implementations.Transform;
using CorgEng.EntityComponentSystem.Systems;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Rendering.Cameras.Isometric;
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

        private static List<IEntity> playerEntities = new List<IEntity>();

        //Contains camera logic which only the client knows about
        public override EntitySystemFlags SystemFlags { get; } = EntitySystemFlags.CLIENT_SYSTEM;

        public override void SystemSetup(IWorld world)
        {
            RegisterLocalEvent<CameraComponent, MoveEvent>(OnCameraMoved);
            RegisterLocalEvent<CameraComponent, ComponentAddedEvent>(OnComponentAdded);
            RegisterLocalEvent<CameraComponent, ComponentRemovedEvent>(OnComponentRemoved);
            RegisterGlobalEvent<MouseScrollEvent>(OnMouseScroll);
            RegisterLocalEvent<CameraComponent, MouseScrollEvent>(OnMouseScrolled);
            RegisterLocalEvent<CameraComponent, InitialiseNetworkedEntityEvent>((entity, component, signal) => {
                if (component.Camera == null)
                    component.Camera = CorgEngMain.MainCamera as IIsometricCamera;
            });
        }

        private void OnCameraMoved(IEntity entity, CameraComponent cameraComponent, MoveEvent moveEvent)
        {
            cameraComponent.Camera.X = moveEvent.NewPosition.X;
            cameraComponent.Camera.Y = moveEvent.NewPosition.Y;
        }

        public void OnComponentRemoved(IEntity entity, CameraComponent cameraComponent, ComponentRemovedEvent componentRemovedEvent)
        {
            if (componentRemovedEvent.Component != cameraComponent)
                return;
            playerEntities.Remove(entity);
        }

        public void OnComponentAdded(IEntity entity, CameraComponent cameraComponent, ComponentAddedEvent componentAddEvent)
        {
            if (componentAddEvent.Component != cameraComponent)
                return;
            playerEntities.Add(entity);
        }

        //cheese
        public void OnMouseScroll(MouseScrollEvent scrollEvent)
        {
            foreach (IEntity entity in playerEntities)
            {
                scrollEvent.Raise(entity);
            }
        }

        public void OnMouseScrolled(IEntity entity, CameraComponent cameraComponent, MouseScrollEvent scrollEvent)
        {
            cameraComponent.Camera.Width -= (float)scrollEvent.ScrollDelta;
            cameraComponent.Camera.Height -= (float)scrollEvent.ScrollDelta;
        }

    }
}

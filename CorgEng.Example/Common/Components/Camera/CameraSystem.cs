using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Implementations.Transform;
using CorgEng.EntityComponentSystem.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Example.Common.Components.Camera
{
    public class CameraSystem : EntitySystem
    {

        public override void SystemSetup()
        {
            RegisterLocalEvent<CameraComponent, MoveEvent>(OnCameraMoved);
        }

        private void OnCameraMoved(Entity entity, CameraComponent cameraComponent, MoveEvent moveEvent)
        {
            cameraComponent.Camera.X = moveEvent.NewPosition.X;
            cameraComponent.Camera.Y = moveEvent.NewPosition.Y;
        }

    }
}

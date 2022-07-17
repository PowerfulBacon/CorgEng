using CorgEng.Core;
using CorgEng.EntityComponentSystem.Systems;
using CorgEng.GenericInterfaces.Rendering.Cameras.Isometric;
using CorgEng.InputHandling.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Example.Modules.CameraScroll
{
    internal class CameraScrollSystem : EntitySystem
    {

        public static IIsometricCamera IsometricCamera;

        public override EntitySystemFlags SystemFlags => EntitySystemFlags.CLIENT_SYSTEM;

        public override void SystemSetup()
        {
            RegisterGlobalEvent<MouseScrollEvent>(OnMouseScroll);
        }

        private void OnMouseScroll(MouseScrollEvent mouseScrollEvent)
        {
            float amount = (float)mouseScrollEvent.ScrollDelta * -2.0f;
            if (amount < 0)
            {
                amount = 1.0f / (-amount);
            }
            IsometricCamera.Height *= amount;
            IsometricCamera.Width *= amount;
        }

    }
}

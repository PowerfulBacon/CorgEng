using CorgEng.EntityComponentSystem.Components;
using CorgEng.GenericInterfaces.ContentLoading;
using CorgEng.GenericInterfaces.Rendering.Cameras.Isometric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Example.Common.Components.Camera
{
    public class CameraComponent : Component
    {

        public IIsometricCamera Camera { get; internal set; }

        public CameraComponent()
        { }

        public CameraComponent(IIsometricCamera camera)
        {
            Camera = camera;
        }

        public override bool SetProperty(string name, IPropertyDef property)
        {
            return false;
        }
    }
}

using CorgEng.EntityComponentSystem.Components;
using CorgEng.EntityComponentSystem.Implementations.Transform;
using CorgEng.World.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.IconSmoothing.Components
{
    public class SmoothIconComponent : TrackComponent
    {

        private TransformComponent? _transformComponent;

        public TransformComponent TransformComponent
        {
            get
            {
                if (_transformComponent == null)
                {
                    _transformComponent = Parent.GetComponent<TransformComponent>();
                }
                return _transformComponent;
            }
        }

        public SmoothIconComponent() : base("_icon_smoothing")
        { }

    }
}

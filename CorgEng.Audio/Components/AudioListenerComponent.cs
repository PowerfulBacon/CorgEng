using CorgEng.EntityComponentSystem.Components;
using CorgEng.EntityComponentSystem.Implementations.Transform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Audio.Components
{
    public class AudioListenerComponent : Component
    {

        private TransformComponent? _transformComponent;

        internal TransformComponent Transform
        {
            get
            {
                if (_transformComponent == null)
                {
                    _transformComponent = Parent.GetComponent<TransformComponent>();
                }
                return _transformComponent;
            }
            set
            {
                if (value != null)
                    throw new Exception("Cannot set transform component.");
                _transformComponent = null;
            }
        }

    }
}

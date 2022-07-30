using CorgEng.EntityComponentSystem.Implementations.Transform;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.World.Components
{
    public class TrackComponent : IComponent
    {

        public bool IsSynced => false;

        public IEntity Parent { get; set; }

        public string Key { get; set; }

        /// <summary>
        /// Get the transform linked to this component
        /// </summary>
        private TransformComponent _storedTransform;
        public TransformComponent Transform {
            get {
                if(_storedTransform == null)
                    _storedTransform = Parent.GetComponent<TransformComponent>();
                return _storedTransform;
            }
        }

        public TrackComponent(string key)
        {
            Key = key;
        }

    }
}

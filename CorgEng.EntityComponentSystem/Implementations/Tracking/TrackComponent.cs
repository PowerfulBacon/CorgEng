using CorgEng.EntityComponentSystem.Components;
using CorgEng.EntityComponentSystem.Implementations.Transform;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.GenericInterfaces.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.World.Components
{
    public class TrackComponent : Component, IWorldTrackComponent
    {

        public string Key { get; set; }

        /// <summary>
        /// Get the transform linked to this component
        /// </summary>
        private TransformComponent _storedTransform;
        public virtual TransformComponent Transform {
            get {
                if(_storedTransform == null)
                    _storedTransform = Parent.GetComponent<TransformComponent>();
                return _storedTransform;
            }
        }

        public int ContentsIndexPosition { get; set; } = -1;

        public IVector<int> ContentsLocation { get; set; } = null;

        public TrackComponent(string key)
        {
            Key = key;
        }

    }
}

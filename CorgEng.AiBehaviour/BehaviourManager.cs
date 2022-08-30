using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Implementations.Transform;
using CorgEng.GenericInterfaces.AiBehaviours;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.AiBehaviour
{
    internal class BehaviourManager : IBehaviourManager
    {

        public bool Thinking { get; set; }

        private TransformComponent _transform;
        public IVector<float> Position => _transform.Position;

        /// <summary>
        /// The pawn entity we are attached to
        /// </summary>
        private IEntity _pawnEntity;
        public IEntity PawnEntity
        {
            get => _pawnEntity;
            set {
                //Set the pawn entity
                _pawnEntity = value;
                //Get its transform component
                _transform = _pawnEntity.GetComponent<TransformComponent>();
            }
        }

        /// <summary>
        /// The behaviour managers memory
        /// </summary>
        private Dictionary<string, object> memStore = new Dictionary<string, object>();

        /// <summary>
        /// The root behaviour node
        /// </summary>
        private BehaviourRoot root = new BehaviourRoot();

        /// <summary>
        /// Setup the behaviour manager
        /// </summary>
        /// <param name="pawnEntity"></param>
        public BehaviourManager(IEntity pawnEntity)
        {
            PawnEntity = pawnEntity;
        }

        public T? GetMemory<T>(string memory)
        {
            if (!memStore.ContainsKey(memory))
                return default;
            return (T)memStore[memory];
        }

        public void SetMemory<T>(string memory, T? value)
        {
            if (memStore.ContainsKey(memory))
            {
                if (value == null)
                    memStore.Remove(memory);
                else
                    memStore[memory] = value;
            }
            else if(value != null)
            {
                memStore.Add(memory, value);
            }
        }

        public async Task Process()
        {
            //Start thinking
            Thinking = true;
            //Process the root
            await root.Action(this);
        }

    }
}

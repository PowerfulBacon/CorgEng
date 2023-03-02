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

        //Are we currently processing? Prevents AIs from having multiple AI trees thinking
        //since they run asynchronously.
        public bool Thinking { get; set; }

        //Cached transform attached to the behaviour manager
        private TransformComponent _transform;


        public IVector<float> Position => _transform.Position.Value;

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
        /// Currently running action
        /// </summary>
        public IBehaviourAction? CurrentAction { get; set; }

        /// <summary>
        /// The behaviour managers permanent memory
        /// </summary>
        private Dictionary<string, object> permanentMemoryStore = new Dictionary<string, object>();

        /// <summary>
        /// The behaviour managers memory
        /// </summary>
        private Dictionary<string, object> memStore = new Dictionary<string, object>();

        /// <summary>
        /// The root behaviour node
        /// </summary>
        internal BehaviourRoot root = new BehaviourRoot();

        /// <summary>
        /// Setup the behaviour manager
        /// </summary>
        /// <param name="pawnEntity"></param>
        public BehaviourManager(IEntity pawnEntity)
        {
            PawnEntity = pawnEntity;
        }

        public async Task Process()
        {
            //Start thinking
            Thinking = true;
            //Process the root9
            await root.Action(this);
            //Flush the temporary memory store
            memStore.Clear();
            //Finish thinking
            Thinking = false;
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

        public T? GetPermanentMemory<T>(string memory)
        {
            if (!permanentMemoryStore.ContainsKey(memory))
                return default;
            return (T)permanentMemoryStore[memory];
        }

        public void SetPermanentMemory<T>(string memory, T? value)
        {
            if (permanentMemoryStore.ContainsKey(memory))
            {
                if (value == null)
                    permanentMemoryStore.Remove(memory);
                else
                    permanentMemoryStore[memory] = value;
            }
            else if (value != null)
            {
                permanentMemoryStore.Add(memory, value);
            }
        }

    }
}

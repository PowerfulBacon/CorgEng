using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Components;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.EntityComponentSystem.Implementations.Deletion;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace CorgEng.EntityComponentSystem.Entities
{

    public class Entity : IEntity
    {

        internal delegate void InternalSignalHandleDelegate(IEntity entity, IEvent signal, bool synchronous);

        [UsingDependency]
        private static ILogger Logger;

        /// <summary>
        /// The identifier for the entity. Used to find a specific entity.
        /// </summary>
        public uint Identifier { get; private set; }

        /// <summary>
        /// List of all components attached to this entity
        /// </summary>
        public List<IComponent> Components { get; } = new List<IComponent>();

        /// <summary>
        /// The index of this entity in the contents array of the parent.
        /// </summary>
        public int ContentsIndex { get; set; } = -1;

        /// <summary>
        /// The location of the contents that we are stored at
        /// </summary>
        public IVector<int> ContentsLocation { get; set; }

        /// <summary>
        /// Components register themselves to this when needed, and this gets fired off to the component
        /// which then can fire off itself to the system.
        /// </summary>
        internal Dictionary<Type, List<InternalSignalHandleDelegate>> EventListeners { get; set; } = null;

        /// <summary>
        /// Name of the entity definition
        /// </summary>
        public string DefinitionName { get; set; }

        public Entity()
        {
            Identifier = EntityManager.GetNewEntityId();
            EntityManager.RegisterEntity(this);
            //Entities are deletable by default
            AddComponent(new DeleteableComponent());
        }

        public Entity(uint identifier)
        {
            Identifier = identifier;
            EntityManager.RegisterEntity(this);
            //Entities are deletable by default
            AddComponent(new DeleteableComponent());
        }

        ~Entity()
        {
            Interlocked.Increment(ref EntityManager.GarbageCollectionCount);
            //Debug
            //Logger.WriteLine($"Entity GC'd. {EntityManager.GarbageCollectionCount}/{EntityManager.DeletionCount}", LogType.TEMP);
        }

        /// <summary>
        /// Add a component to the specified entity
        /// </summary>
        /// <param name="component">A reference to the component to be added</param>
        public void AddComponent(IComponent component)
        {
            lock (Components)
            {
                Components.Add(component);
                component.OnComponentAdded(this);
            }
        }

        /// <summary>
        /// Remove a component from the specified entity.
        /// </summary>
        /// <param name="component">The reference to the component to remove</param>
        public void RemoveComponent(IComponent component, bool networked)
        {
            lock (Components)
            {
                component.OnComponentRemoved(this);
                Components.Remove(component);
            }
        }

        /// <summary>
        /// Internal method for handling signals.
        /// </summary>
        /// <param name="signal"></param>
        public void HandleSignal(IEvent signal, bool synchronous = false)
        {
            //Verify that this signal is being listened for
            if (EventListeners == null)
                return;
            if (!EventListeners.ContainsKey(signal.GetType()))
                return;
            //Fetch the registered signal handlers
            List<InternalSignalHandleDelegate> signalHandleDelegates = EventListeners[signal.GetType()];
            //Call the signals
            //Inverse for loop to account for removal of components and signals during iteration
            for (int i = signalHandleDelegates.Count - 1; i >= 0; i = Math.Min(i - 1, signalHandleDelegates.Count - 1))
            {
                InternalSignalHandleDelegate internalSignalHandler = signalHandleDelegates[i];
                internalSignalHandler.Invoke(this, signal, synchronous);
            }
        }

        public T GetComponent<T>()
        {
            //Get derived types too
            lock (Components)
            {
                foreach (IComponent component in Components)
                {
                    if (component is T componentAsType)
                        return componentAsType;
                }
            }
            throw new Exception($"Component of type {typeof(T)} was not found on Entity {this}");
        }

        public override string ToString()
        {
            return $"Entity{Identifier}";
        }

        public bool HasComponent<T>()
        {
            //Get derived types too
            lock (Components)
            {
                foreach (IComponent component in Components)
                {
                    if (component is T)
                        return true;
                }
            }
            return false;
        }

        public T? FindComponent<T>()
        {
            throw new NotImplementedException();
        }
    }
}

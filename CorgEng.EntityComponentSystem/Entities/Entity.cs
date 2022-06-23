using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Components;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace CorgEng.EntityComponentSystem.Entities
{

    public class Entity : IEntity
    {

        internal delegate void InternalSignalHandleDelegate(IEntity entity, IEvent signal);

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
        /// Components register themselves to this when needed, and this gets fired off to the component
        /// which then can fire off itself to the system.
        /// </summary>
        internal Dictionary<Type, List<InternalSignalHandleDelegate>> EventListeners { get; set; } = null;

        public Entity()
        {
            Identifier = EntityManager.GetNewEntityId();
            EntityManager.RegisterEntity(this);
        }

        public Entity(uint identifier)
        {
            Identifier = identifier;
            EntityManager.RegisterEntity(this);
        }

        ~Entity()
        {
            Interlocked.Increment(ref EntityManager.GarbageCollectionCount);
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
        public void HandleSignal(IEvent signal)
        {
            //Verify that this signal is being listened for
            if (EventListeners == null)
                return;
            if (!EventListeners.ContainsKey(signal.GetType()))
                return;
            //Fetch the registered signal handlers
            List<InternalSignalHandleDelegate> signalHandleDelegates = EventListeners[signal.GetType()];
            //Call the signals
            foreach (InternalSignalHandleDelegate internalSignalHandler in signalHandleDelegates)
            {
                internalSignalHandler.Invoke(this, signal);
            }
        }

    }
}

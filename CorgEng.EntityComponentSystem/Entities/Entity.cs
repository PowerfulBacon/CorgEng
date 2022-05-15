using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Components;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.GenericInterfaces.Logging;
using System;
using System.Collections.Generic;
using System.Threading;

namespace CorgEng.EntityComponentSystem.Entities
{
    public class Entity
    {

        internal delegate void InternalSignalHandleDelegate(Entity entity, Event signal);

        private static int GarbageCollectionCount = 0;

        private static int DeletionCount = 0;

        /// <summary>
        /// Amount of created entities
        /// </summary>
        private static int CreatedEntityCount = 0;

        /// <summary>
        /// The identifier for the entity. Used to find a specific entity.
        /// </summary>
        public int Identifier { get; private set; }

        /// <summary>
        /// List of all components attached to this entity
        /// </summary>
        public List<Component> Components { get; } = new List<Component>();

        /// <summary>
        /// Components register themselves to this when needed, and this gets fired off to the component
        /// which then can fire off itself to the system.
        /// </summary>
        internal Dictionary<Type, List<InternalSignalHandleDelegate>> EventListeners { get; set; } = null;

        public Entity()
        {
            Identifier = CreatedEntityCount;
            EntityManager.RegisterEntity(this);
        }

        /// <summary>
        /// Delete this entity, remove all references to it.
        /// Triggered when an EntityDeletedEvent is raised against an entity.
        /// 
        /// EntityDeletedEvent
        /// -> Networking
        /// -> Deletion System Raised
        /// -> Delete() method
        /// -> Component local removal + EntityManager removal
        /// </summary>
        internal void Delete()
        {
            Interlocked.Increment(ref DeletionCount);
            EntityManager.RemoveEntity(this);
            //Remove all components
            for (int i = Components.Count; i >= 0; i--)
            {
                RemoveComponent(Components[i], false);
            }
        }

        ~Entity()
        {
            Interlocked.Increment(ref GarbageCollectionCount);
        }

        /// <summary>
        /// Add a component to the specified entity
        /// </summary>
        /// <param name="component">A reference to the component to be added</param>
        public void AddComponent(Component component)
        {
            Components.Add(component);
            component.OnComponentAdded(this);
        }

        /// <summary>
        /// Remove a component from the specified entity.
        /// </summary>
        /// <param name="component">The reference to the component to remove</param>
        public void RemoveComponent(Component component, bool networked)
        {
            component.OnComponentRemoved(this, networked);
            Components.Remove(component);
        }

        /// <summary>
        /// Internal method for handling signals.
        /// </summary>
        /// <param name="signal"></param>
        internal void HandleSignal(Event signal)
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

﻿using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Components;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.EntityComponentSystem.Implementations.Deletion;
using CorgEng.EntityComponentSystem.Systems;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.UtilityTypes.Monads;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using static CorgEng.GenericInterfaces.EntityComponentSystem.IEntity;

namespace CorgEng.EntityComponentSystem.Entities
{

    public class Entity : WorldObject, IEntity
    {

        [UsingDependency]
        private static ILogger Logger;

        /// <summary>
        /// The identifier for the entity. Used to find a specific entity.
        /// </summary>
        public uint Identifier { get; private set; }

        /// <summary>
        /// The flags relating to this entity. Indicate if it has been initialised, destroyed etc.
        /// </summary>
        public EntityFlags EntityFlags { get; set; } = EntityFlags.NONE;

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
        public Dictionary<Type, List<InternalSignalHandleDelegate>> EventListeners { get; set; } = null;

        /// <summary>
        /// Name of the entity definition
        /// </summary>
        public string DefinitionName { get; set; }

        internal Entity(IWorld world) : base(world)
        {
            Identifier = world.EntityManager.GetNewEntityId();
            world.EntityManager.RegisterEntity(this);
            //Entities are deletable by default
            AddComponent(new DeleteableComponent());
        }

        internal Entity(IWorld world, uint identifier) : base(world)
        {
            Identifier = identifier;
            world.EntityManager.RegisterEntity(this);
            //Entities are deletable by default
            AddComponent(new DeleteableComponent());
        }

        ~Entity()
        {
            Interlocked.Increment(ref ((EntityManager)world.EntityManager).GarbageCollectionCount);
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
#if DEBUG
                foreach (Component ecomp in Components)
                {
                    if (ecomp.GetType() == component.GetType())
                        throw new Exception("Component added twice");
                }
#endif
                Components.Add(component);
                world.ComponentSignalInjector.OnComponentAdded(component, this);
            }
        }

        /// <summary>
        /// Remove a component from the specified entity.
        /// </summary>
        /// <param name="component">The reference to the component to remove</param>
        public void RemoveComponent(IComponent component, bool networked, bool silent = false)
        {
            lock (Components)
            {
                world.ComponentSignalInjector.OnComponentRemoved(component, this, silent);
                Components.Remove(component);
            }
        }

        /// <summary>
        /// Internal method for handling signals.
        /// </summary>
        /// <param name="signal"></param>
        public void HandleSignal(IEvent signal, bool synchronous, string callingFile, string callingMember, int callingLine)
        {
            //Verify that this signal is being listened for
            if (EventListeners == null)
                return;
            if (EventListeners.TryGetValue(signal.GetType(), out List<InternalSignalHandleDelegate> signalHandleDelegates))
            {
                //Call the signals
                //Inverse for loop to account for removal of components and signals during iteration
                for (int i = signalHandleDelegates.Count - 1; i >= 0; i = Math.Min(i - 1, signalHandleDelegates.Count - 1))
                {
                    InternalSignalHandleDelegate internalSignalHandler = signalHandleDelegates[i];
                    internalSignalHandler.Invoke(this, signal, synchronous, callingFile, callingMember, callingLine);
                }
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
            //Get derived types too
            lock (Components)
            {
                foreach (IComponent component in Components)
                {
                    if (component is T located)
                        return located;
                }
            }
            return default;
        }

        public bool TryGetComponent<T>(out T component)
        {
            //Get derived types too
            lock (Components)
            {
                foreach (IComponent _component in Components)
                {
                    if (_component is T componentAsT)
                    {
                        component = componentAsT;
                        return true;
                    }
                }
            }
            component = default(T);
            return false;
        }

        public Result<T> TryGetComponent<T>()
        {
            //Get derived types too
            lock (Components)
            {
                foreach (IComponent _component in Components)
                {
                    if (_component is T componentAsT)
                    {
                        return new Result<T>(componentAsT);
                    }
                }
            }
            return new Failure<T>();
        }

        public void AddEventListener(Type eventType, InternalSignalHandleDelegate eventHandler)
        {
            if (EventListeners == null)
                EventListeners = new Dictionary<Type, List<InternalSignalHandleDelegate>>();
            if (EventListeners.ContainsKey(eventType))
                EventListeners[eventType].Add(eventHandler);
            else
                EventListeners.Add(eventType, new List<InternalSignalHandleDelegate>() { eventHandler });
        }

        public void RemoveEventListenter(Type eventType, InternalSignalHandleDelegate eventHandler)
        {
            if (EventListeners[eventType].Contains(eventHandler))
            {
                EventListeners[eventType].Remove(eventHandler);
            }
        }
    }
}

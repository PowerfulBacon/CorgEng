using CorgEng.Core.Modules;
using CorgEng.Core;
using CorgEng.GenericInterfaces.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using static CorgEng.EntityComponentSystem.Systems.EntitySystem;
using static CorgEng.GenericInterfaces.EntityComponentSystem.IEntitySystemManager;

namespace CorgEng.EntityComponentSystem.Systems
{

    internal class EntitySystemManager : IEntitySystemManager
    {

        [UsingDependency]
        protected static ILogger Logger;

        /// <summary>
        /// A static list of all entity systems in use
        /// </summary>
        public ConcurrentDictionary<Type, EntitySystem> EntitySystems { get; }
            = new ConcurrentDictionary<Type, EntitySystem>();

        /// <summary>
        /// has setup been completed?
        /// </summary>
        public bool SetupCompleted { get; private set; } = false;

        /// <summary>
        /// Actions to run after setup
        /// </summary>
        public event Action postSetupAction;

        /// <summary>
        /// Matches component type to types of registered events
        /// </summary>
        internal Dictionary<Type, List<Type>> RegisteredEvents = new Dictionary<Type, List<Type>>();

        /// <summary>
        /// Matches event and component types to registered signal handlers on systems
        /// </summary>
        internal Dictionary<EventComponentPair, List<SystemEventHandlerDelegate>> RegisteredSystemSignalHandlers { get; } = new Dictionary<EventComponentPair, List<SystemEventHandlerDelegate>>();

        private EntitySystemThreadManager entitySystemThreadManager = new EntitySystemThreadManager(4);

        private IWorld world;

        public EntitySystemManager(IWorld world)
        {
            this.world = world;
        }

        ~EntitySystemManager()
        {
            TerminateSubsystems();
            EntitySystems.Clear();
            entitySystemThreadManager.Cleanup();
        }

        /// <summary>
        /// Called when the attached world process is created.
        /// Creates all System types and tracks the to prevent GC
        /// </summary>
        internal void CreateAllSystems()
        {
            Logger?.WriteLine($"Setting up systems...", LogType.LOG);
            //Locate all system types using reflection.
            //Note that we need all systems in all loaded modules
            IEnumerable<Type> locatedSystems = CorgEngMain.LoadedAssemblyModules
                .SelectMany(assembly => assembly.GetTypes()
                .Where(type => typeof(EntitySystem).IsAssignableFrom(type) && !type.IsAbstract));
            int setupAmount = 0;
            locatedSystems.Select((type) =>
            {
                Logger?.WriteLine($"Initializing {type}...", LogType.LOG);
                EntitySystem createdSystem = (EntitySystem)type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, CallingConventions.HasThis, new Type[0], null).Invoke(new object[0]);
                createdSystem.JoinWorld(world);
                createdSystem.JoinEntitySystemManager(entitySystemThreadManager);
                EntitySystems.TryAdd(createdSystem.GetType(), createdSystem);
                return createdSystem;
            })
                // Very important that we fully resolve the enumerator and don't evaluate it lazilly
                .ToList()
                // Now do the foreach after they have all been created
                .ForEach(entitySystem =>
                {
                    setupAmount++;
                    entitySystem.SystemSetup(world);
                });
            SetupCompleted = true;
            // Run post-setup actions
            postSetupAction?.Invoke();
            postSetupAction = null;
            //Trigger the event when this is all done and loaded
            CorgEngMain.OnReadyEvents += () =>
            {
                new GameReadyEvent().RaiseGlobally(world);
            };
            Logger?.WriteLine($"Successfully created and setup {setupAmount} systems!", LogType.LOG);
        }

        /// <summary>
        /// Gets a specific entity system
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetSingleton<T>()
        {
            return (T)(object)EntitySystems[typeof(T)];
        }

        /// <summary>
        /// Shut down all subsystems that are running inside this entity system
        /// manager.
        /// </summary>
        internal void TerminateSubsystems()
        {
            new GameClosedEvent().RaiseGlobally(world);
        }

        public void RegisterEventType(Type componentType, Type eventType)
        {
            lock (RegisteredEvents)
            {
                if (!RegisteredEvents.ContainsKey(componentType))
                    RegisteredEvents.Add(componentType, new List<Type>());
                if (!RegisteredEvents[componentType].Contains(eventType))
                    RegisteredEvents[componentType].Add(eventType);
            }
        }

        //TODO: Does this work?
        //TODO: Create unit tests for the expected behaviour of unregister event type.
        public void UnregisterEventType(Type componentType, Type eventType)
        {
            lock (RegisteredEvents)
            {
                if (!RegisteredEvents.ContainsKey(componentType))
                    throw new Exception("Attempted to unregister an event that was not present on the target entity system. (Component is not registered, are you using the right generic types?)");
                if (!RegisteredEvents[componentType].Contains(eventType))
                    throw new Exception("Attempted to unregister an event that was not present on the target entity system. (Event was not registered, are you using the right generic types?)");
            }
        }

        public IEnumerable<Type> GetRegisteredEventTypes(Type componentType)
        {
            lock (RegisteredEvents)
            {
                if (RegisteredEvents.ContainsKey(componentType))
                    return RegisteredEvents[componentType];
            }
            return Enumerable.Empty<Type>();
        }

        public void RegisterSystemEventHandler(EventComponentPair registeredEventComponentType, SystemEventHandlerDelegate eventAction)
        {
            lock (RegisteredSystemSignalHandlers)
            {
                if (!RegisteredSystemSignalHandlers.ContainsKey(registeredEventComponentType))
                    RegisteredSystemSignalHandlers.Add(registeredEventComponentType, new List<SystemEventHandlerDelegate>());
                RegisteredSystemSignalHandlers[registeredEventComponentType].Add(eventAction);
            }
        }

        public void UnregisterSystemEventHandler(EventComponentPair registeredEventComponentType, SystemEventHandlerDelegate eventAction)
        {
            lock (RegisteredSystemSignalHandlers)
            {
                RegisteredSystemSignalHandlers[registeredEventComponentType].Remove(eventAction);
                if (RegisteredSystemSignalHandlers[registeredEventComponentType].Count == 0)
                {
                    RegisteredSystemSignalHandlers.Remove(registeredEventComponentType);
                }
            }
        }

        public List<SystemEventHandlerDelegate> GetRegisteredSystemEventHandlers(EventComponentPair registeredEventComponentType)
        {
            lock (RegisteredSystemSignalHandlers)
            {
                if (!RegisteredSystemSignalHandlers.ContainsKey(registeredEventComponentType))
                    return new List<SystemEventHandlerDelegate>();
                return RegisteredSystemSignalHandlers[registeredEventComponentType];
            }
        }
    }
}

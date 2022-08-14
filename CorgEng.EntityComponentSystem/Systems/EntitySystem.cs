using CorgEng.Core;
using CorgEng.Core.Dependencies;
using CorgEng.Core.Modules;
using CorgEng.EntityComponentSystem.Components;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.GenericInterfaces.ContentLoading;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Networking.Config;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Systems
{
    public abstract class EntitySystem
    {

        /// <summary>
        /// Internal global event component specifically for handling global signals
        /// </summary>
        internal class GlobalEventComponent : Component
        {
        }

        [UsingDependency]
        private static ILogger Logger;

        /// <summary>
        /// Network config, null if the application doesn't have networking capabilities.
        /// </summary>
        [UsingDependency]
        protected static INetworkConfig NetworkConfig;

        internal delegate void SystemEventHandlerDelegate(IEntity entity, IComponent component, IEvent signal, bool synchronous);

        /// <summary>
        /// Matches event and component types to registered signal handlers on systems
        /// </summary>
        internal static Dictionary<EventComponentPair, List<SystemEventHandlerDelegate>> RegisteredSystemSignalHandlers { get; } = new Dictionary<EventComponentPair, List<SystemEventHandlerDelegate>>();

        /// <summary>
        /// Wait handler. This will cause the thread to pause until an event that needs handling is raised.
        /// </summary>
        protected readonly AutoResetEvent waitHandle = new AutoResetEvent(false);

        protected volatile bool isWaiting = false;

        /// <summary>
        /// A queue of actions that other parts of the code are waiting for the action's completion.
        /// These will be execution before anything in the invokation queue.
        /// </summary>
        protected readonly ConcurrentQueue<Action> priorityInvokationQueue = new ConcurrentQueue<Action>();

        /// <summary>
        /// The invokation queue. A queue of actions that need to be triggered (Raised events)
        /// </summary>
        protected readonly ConcurrentQueue<Action> invokationQueue = new ConcurrentQueue<Action>();

        /// <summary>
        /// A static list of all entity systems in use
        /// </summary>
        private static List<EntitySystem> EntitySystems = new List<EntitySystem>();

        /// <summary>
        /// The flags of this system
        /// </summary>
        public abstract EntitySystemFlags SystemFlags { get; }

        /// <summary>
        /// If we have been targeted for a kill
        /// </summary>
        protected bool assassinated = false;

        public EntitySystem()
        {
            Thread thread = new Thread(SystemThread);
            thread.Name = $"{this} thread";
            thread.Start();
            //Register the global signal to handle closing the game
            RegisterGlobalEvent((GameClosedEvent e) => { });
        }

        public abstract void SystemSetup();

        /// <summary>
        /// Called when the ECS module is loaded.
        /// Creates all System types and tracks the to prevent GC
        /// </summary>
        [ModuleLoad]
        private static void CreateAllSystems()
        {
            Logger?.WriteLine($"Setting up systems...", LogType.LOG);
            //Locate all system types using reflection.
            //Note that we need all systems in all loaded modules
            IEnumerable<Type> locatedSystems = CorgEngMain.LoadedAssemblyModules
                .SelectMany(assembly => assembly.GetTypes()
                .Where(type => typeof(EntitySystem).IsAssignableFrom(type) && !type.IsAbstract));
            Parallel.ForEach(locatedSystems, (type) => {
                Logger?.WriteLine($"Initializing {type}...", LogType.LOG);
                EntitySystem createdSystem = (EntitySystem)type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, CallingConventions.HasThis, new Type[0], null).Invoke(new object[0]);
                createdSystem.SystemSetup();
                EntitySystems.Add(createdSystem);
            });
            Logger?.WriteLine($"Successfully created and setup all systems!", LogType.LOG);
        }

        [ModuleTerminate]
        private static void TerminateSubsystems()
        {
            new GameClosedEvent().RaiseGlobally();
        }

        /// <summary>
        /// The system thread. Waits until an invokation is required and then triggers it
        /// on the system's thread.
        /// </summary>
        protected virtual void SystemThread()
        {
            while (!CorgEngMain.Terminated && !assassinated)
            {
                //Wait until we are awoken again
                if (invokationQueue.Count == 0 && priorityInvokationQueue.Count == 0)
                {
                    isWaiting = true;
                    waitHandle.WaitOne();
                    isWaiting = false;
                }
                Action firstInvokation;
                if (priorityInvokationQueue.Count != 0)
                    priorityInvokationQueue.TryDequeue(out firstInvokation);
                else
                    invokationQueue.TryDequeue(out firstInvokation);
                if (firstInvokation != null)
                {
                    try
                    {
                        //Invoke the provided action
                        firstInvokation();
                    }
                    catch (Exception e)
                    {
                        Logger?.WriteLine(e, LogType.ERROR);
                    }
                }

            }
            //Terminated
            Logger?.WriteLine($"Terminated EntitySystem thread: {this}", LogType.LOG);
        }

        /// <summary>
        /// Kills this and only this system
        /// </summary>
        public void Kill()
        {
            //Effective
            assassinated = true;
            //Tell the system to process its death
            if (isWaiting)
                waitHandle.Set();
        }

        /// <summary>
        /// Registers a signal to a global event.
        /// </summary>
        /// <typeparam name="GEvent">The type of the global event to subscribe to.</typeparam>
        /// <param name="eventHandler">The method to be invoked when the global event is raised.</param>
        public void RegisterGlobalEvent<GEvent>(Action<GEvent> eventHandler)
            where GEvent : IEvent
        {
            //Register the component to recieve the target event on the event manager
            lock (EventManager.RegisteredEvents)
            {
                if (!EventManager.RegisteredEvents.ContainsKey(typeof(GlobalEventComponent)))
                    EventManager.RegisteredEvents.Add(typeof(GlobalEventComponent), new List<Type>());
                if (!EventManager.RegisteredEvents[typeof(GlobalEventComponent)].Contains(typeof(GEvent)))
                    EventManager.RegisteredEvents[typeof(GlobalEventComponent)].Add(typeof(GEvent));
            }
            //Register the system to receieve the event
            EventComponentPair eventComponentPair = new EventComponentPair(typeof(GEvent), typeof(GlobalEventComponent));
            lock (RegisteredSystemSignalHandlers)
            {
                if (!RegisteredSystemSignalHandlers.ContainsKey(eventComponentPair))
                    RegisteredSystemSignalHandlers.Add(eventComponentPair, new List<SystemEventHandlerDelegate>());
                RegisteredSystemSignalHandlers[eventComponentPair].Add((IEntity entity, IComponent component, IEvent signal, bool synchronous) =>
                {
                    ConcurrentQueue<Action> targetQueue = synchronous ? priorityInvokationQueue : invokationQueue;
                    AutoResetEvent synchronousWaitEvent = synchronous ? new AutoResetEvent(false) : null;
                    targetQueue.Enqueue(() =>
                    {
                        //Check if we don't process
                        if (NetworkConfig != null
                            && NetworkConfig.NetworkingActive
                            && ((SystemFlags & EntitySystemFlags.HOST_SYSTEM) == 0 || !NetworkConfig.ProcessServerSystems)
                            && ((SystemFlags & EntitySystemFlags.CLIENT_SYSTEM) == 0 || !NetworkConfig.ProcessClientSystems))
                            return;
                        eventHandler.Invoke((GEvent)signal);
                        //If we were synchronous, indicate that the queuer can continue
                        if (synchronous)
                        {
                            synchronousWaitEvent.Set();
                        }
                    });
                    //Wake up the system if its sleeping
                    if(isWaiting)
                        waitHandle.Set();
                    //If this event is synchronous, wait for completion
                    if (synchronous)
                        synchronousWaitEvent.WaitOne();
                });
            }
        }

        private static IEnumerable<Type> TypeCache;

        /// <summary>
        /// Register to a local event
        /// </summary>
        public void RegisterLocalEvent<GComponent, GEvent>(Action<IEntity, GComponent, GEvent> eventHandler)
            where GComponent : IComponent
            where GEvent : IEvent
        {
            //Handle assembly cache
            if (TypeCache == null)
            {
                TypeCache = CorgEngMain.LoadedAssemblyModules
                    .SelectMany(assembly => assembly.GetTypes());
            }
            IEnumerable<Type> typesToRegister = TypeCache.Where(type => typeof(GComponent).IsAssignableFrom(type));
            //Determine all types that need to be registered
            foreach (Type typeToRegister in typesToRegister)
            {
                //Register the component to recieve the target event on the event manager
                lock (EventManager.RegisteredEvents)
                {
                    if (!EventManager.RegisteredEvents.ContainsKey(typeToRegister))
                        EventManager.RegisteredEvents.Add(typeToRegister, new List<Type>());
                    if (!EventManager.RegisteredEvents[typeToRegister].Contains(typeof(GEvent)))
                        EventManager.RegisteredEvents[typeToRegister].Add(typeof(GEvent));
                }
                //Register the system to receieve the event
                EventComponentPair eventComponentPair = new EventComponentPair(typeof(GEvent), typeToRegister);
                lock (RegisteredSystemSignalHandlers)
                {
                    if (!RegisteredSystemSignalHandlers.ContainsKey(eventComponentPair))
                        RegisteredSystemSignalHandlers.Add(eventComponentPair, new List<SystemEventHandlerDelegate>());
                    RegisteredSystemSignalHandlers[eventComponentPair].Add((IEntity entity, IComponent component, IEvent signal, bool synchronous) =>
                    {
                        //Check if we don't process
                        if (NetworkConfig != null
                                && NetworkConfig.NetworkingActive
                                && ((SystemFlags & EntitySystemFlags.HOST_SYSTEM) == 0 || !NetworkConfig.ProcessServerSystems)
                                && ((SystemFlags & EntitySystemFlags.CLIENT_SYSTEM) == 0 || !NetworkConfig.ProcessClientSystems))
                            return;
                        ConcurrentQueue<Action> targetQueue = synchronous ? priorityInvokationQueue : invokationQueue;
                        AutoResetEvent synchronousWaitEvent = synchronous ? new AutoResetEvent(false) : null;
                        targetQueue.Enqueue(() =>
                        {
                            eventHandler(entity, (GComponent)component, (GEvent)signal);
                            //If we were synchronous, indicate that the queuer can continue
                            if (synchronous)
                            {
                                synchronousWaitEvent.Set();
                            }
                        });
                        if (isWaiting)
                            waitHandle.Set();
                        //If this event is synchronous, wait for completion
                        if (synchronous)
                            synchronousWaitEvent.WaitOne();
                    });
                }
            }
        }
    }
}

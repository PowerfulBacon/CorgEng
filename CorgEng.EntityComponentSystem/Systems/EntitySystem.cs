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
using System.Runtime.CompilerServices;
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
        protected static ILogger Logger;

        /// <summary>
        /// Network config, null if the application doesn't have networking capabilities.
        /// </summary>
        [UsingDependency]
        protected static INetworkConfig NetworkConfig;

        internal delegate void SystemEventHandlerDelegate(IEntity entity, IComponent component, IEvent signal, bool synchronous, string file, string member, int lineNumber);

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
        protected readonly ConcurrentQueue<InvokationAction> priorityInvokationQueue = new ConcurrentQueue<InvokationAction>();

        /// <summary>
        /// The invokation queue. A queue of actions that need to be triggered (Raised events)
        /// </summary>
        protected readonly ConcurrentQueue<InvokationAction> invokationQueue = new ConcurrentQueue<InvokationAction>();

        /// <summary>
        /// A static list of all entity systems in use
        /// </summary>
        private static ConcurrentDictionary<Type, EntitySystem> EntitySystems = new ConcurrentDictionary<Type, EntitySystem>();

        /// <summary>
        /// The flags of this system
        /// </summary>
        public abstract EntitySystemFlags SystemFlags { get; }

        /// <summary>
        /// If we have been targeted for a kill
        /// </summary>
        protected bool assassinated = false;

        /// <summary>
        /// has setup been completed?
        /// </summary>
        public static bool SetupCompleted = false;

        /// <summary>
        /// Actions to run after setup
        /// </summary>
        public static event Action postSetupAction;

        public EntitySystem()
        {
            Task task = new Task(SystemThread);
            //task.Name = $"{this} thread";
            task.Start();
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
            locatedSystems.Select((type) =>
                {
                    Logger?.WriteLine($"Initializing {type}...", LogType.LOG);
                    EntitySystem createdSystem = (EntitySystem)type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, CallingConventions.HasThis, new Type[0], null).Invoke(new object[0]);
                    EntitySystems.TryAdd(createdSystem.GetType(), createdSystem);
                    return createdSystem;
                })
                // Very important that we fully resolve the enumerator and don't evaluate it lazilly
                .ToList()
                // Now do the foreach after they have all been created
                .ForEach(entitySystem => entitySystem.SystemSetup());
            SetupCompleted = true;
            // Run post-setup actions
            postSetupAction?.Invoke();
            postSetupAction = null;
            //Trigger the event when this is all done and loaded
            CorgEngMain.OnReadyEvents += () => {
                new GameReadyEvent().RaiseGlobally();
            };
            Logger?.WriteLine($"Successfully created and setup all systems!", LogType.LOG);
        }

        /// <summary>
        /// Gets a specific entity system
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetSingleton<T>()
        {
            return (T)(object)EntitySystems[typeof(T)];
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
                    //Protection from concurrency dangers
                    if (invokationQueue.Count == 0 && priorityInvokationQueue.Count == 0)
                    {
                        waitHandle.WaitOne();
                    }
                    isWaiting = false;
                }
                InvokationAction firstInvokation;
                if (priorityInvokationQueue.Count != 0)
                    priorityInvokationQueue.TryDequeue(out firstInvokation);
                else
                    invokationQueue.TryDequeue(out firstInvokation);
                if (firstInvokation != null)
                {
                    try
                    {
                        //Invoke the provided action
                        firstInvokation.Action();
                    }
                    catch (Exception e)
                    {
                        Logger?.WriteLine($"Event Called From: {firstInvokation.CallingMemberName}:{firstInvokation.CallingLineNumber} in {firstInvokation.CallingFile}\n{e}", LogType.ERROR);
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

        private Dictionary<object, SystemEventHandlerDelegate> _globalLinkedHandlers = new Dictionary<object, SystemEventHandlerDelegate>();

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
                SystemEventHandlerDelegate createdEventHandler = (IEntity entity, IComponent component, IEvent signal, bool synchronous, string file, string member, int lineNumber) =>
                {
                    ConcurrentQueue<InvokationAction> targetQueue = synchronous ? priorityInvokationQueue : invokationQueue;
                    AutoResetEvent synchronousWaitEvent = synchronous ? new AutoResetEvent(false) : null;
                    targetQueue.Enqueue(new InvokationAction(() =>
                    {
                        try
                        {
                            //Check if we don't process
                            if (NetworkConfig != null
                                && NetworkConfig.NetworkingActive
                                && ((SystemFlags & EntitySystemFlags.HOST_SYSTEM) == 0 || !NetworkConfig.ProcessServerSystems)
                                && ((SystemFlags & EntitySystemFlags.CLIENT_SYSTEM) == 0 || !NetworkConfig.ProcessClientSystems))
                                return;
                            eventHandler.Invoke((GEvent)signal);
                        }
                        finally
                        {
                            //If we were synchronous, indicate that the queuer can continue
                            if (synchronous)
                            {
                                synchronousWaitEvent.Set();
                                Thread.Yield();
                            }
                        }
                    }, file, member, lineNumber));
                    //Wake up the system if its sleeping
                    if (isWaiting)
                        waitHandle.Set();
                    //If this event is synchronous, wait for completion
                    if (synchronous)
                        synchronousWaitEvent.WaitOne();
                };
                lock (_globalLinkedHandlers)
                {
                    if (_globalLinkedHandlers.ContainsKey(eventHandler))
                        throw new Exception("Attempting to register a global signal handler that is already registered to this system.");
                    _globalLinkedHandlers.Add(eventHandler, createdEventHandler);
                }
                RegisteredSystemSignalHandlers[eventComponentPair].Add(createdEventHandler);
            }
        }

        /// <summary>
        /// Unregister a global event
        /// </summary>
        /// <typeparam name="GEvent"></typeparam>
        /// <param name="eventHandler"></param>
        /// <exception cref="Exception"></exception>
        public void UnregisterGlobalEvent<GEvent>(Action<GEvent> eventHandler)
            where GEvent : IEvent
        {
            //Register the component to recieve the target event on the event manager
            lock (EventManager.RegisteredEvents)
            {
                if (!EventManager.RegisteredEvents.ContainsKey(typeof(GlobalEventComponent)))
                    throw new Exception("Attempted to unregister an event that was not present on the target entity system. (Component is not registered, are you using the right generic types?)");
                if (!EventManager.RegisteredEvents[typeof(GlobalEventComponent)].Contains(typeof(GEvent)))
                    throw new Exception("Attempted to unregister an event that was not present on the target entity system. (Event was not registered, are you using the right generic types?)");
            }
            //Register the system to receieve the event
            EventComponentPair eventComponentPair = new EventComponentPair(typeof(GEvent), typeof(GlobalEventComponent));
            lock (RegisteredSystemSignalHandlers)
            {
                if (!RegisteredSystemSignalHandlers.ContainsKey(eventComponentPair))
                    RegisteredSystemSignalHandlers.Add(eventComponentPair, new List<SystemEventHandlerDelegate>());
                //Hnadle unregistering
                lock (_globalLinkedHandlers)
                {
                    if (!_globalLinkedHandlers.ContainsKey(eventHandler))
                        throw new Exception("Attempting to unregister a global event handler that isn't currently registered.");
                    RegisteredSystemSignalHandlers[eventComponentPair].Remove(_globalLinkedHandlers[eventHandler]);
                    _globalLinkedHandlers.Remove(eventHandler);
                }
            }
        }

        private static IEnumerable<Type> TypeCache;

        private Dictionary<(Type, object), SystemEventHandlerDelegate> _linkedHandlers = new Dictionary<(Type, object), SystemEventHandlerDelegate>();

        /// <summary>
        /// Register to a local event
        /// </summary>
        /// <returns>Returns a reference</returns>
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
            SystemEventHandlerDelegate createdEventHandler = null;
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
                    //Create and return an event handler so that it can be 
                    createdEventHandler = (IEntity entity, IComponent component, IEvent signal, bool synchronous, string callingFile, string callingMember, int callingLine) =>
                    {
                        //Check if we don't process
                        if (NetworkConfig != null
                                && NetworkConfig.NetworkingActive
                                && ((SystemFlags & EntitySystemFlags.HOST_SYSTEM) == 0 || !NetworkConfig.ProcessServerSystems)
                                && ((SystemFlags & EntitySystemFlags.CLIENT_SYSTEM) == 0 || !NetworkConfig.ProcessClientSystems))
                            return;
                        ConcurrentQueue<InvokationAction> targetQueue = synchronous ? priorityInvokationQueue : invokationQueue;
                        AutoResetEvent synchronousWaitEvent = synchronous ? new AutoResetEvent(false) : null;
                        targetQueue.Enqueue(new InvokationAction(() =>
                        {
                            try
                            {
                                eventHandler(entity, (GComponent)component, (GEvent)signal);
                            }
                            catch (Exception e)
                            {
                                throw new Exception($"An exception occurred handling the signal type {typeof(GEvent)} registered on component {typeof(GComponent)} at system {GetType()}.", e);
                            }
                            finally
                            {
                                //If we were synchronous, indicate that the queuer can continue
                                if (synchronous)
                                {
                                    synchronousWaitEvent.Set();
                                }
                            }
                        }, callingFile, callingMember, callingLine));
                        if (isWaiting)
                            waitHandle.Set();
                        //If this event is synchronous, wait for completion
                        if (synchronous)
                            synchronousWaitEvent.WaitOne();
                    };
                    lock (_linkedHandlers)
                    {
                        if (_linkedHandlers.ContainsKey((typeToRegister, eventHandler)))
                            throw new Exception("Attempting to register an event that is already registered.");
                        _linkedHandlers.Add((typeToRegister, eventHandler), createdEventHandler);
                    }
                    RegisteredSystemSignalHandlers[eventComponentPair].Add(createdEventHandler);
                }
            }
        }

        /// <summary>
        /// Allows a signal to be unregistered
        /// </summary>
        /// <typeparam name="GComponent"></typeparam>
        /// <typeparam name="GEvent"></typeparam>
        /// <param name="handlerToRemove"></param>
        public void UnregisterLocalEvent<GComponent, GEvent>(Action<IEntity, GComponent, GEvent> eventHandler)
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
                        throw new Exception("Attempted to unregister an event that was not present on the target entity system. (Component is not registered, are you using the right generic types?)");
                    if (!EventManager.RegisteredEvents[typeToRegister].Contains(typeof(GEvent)))
                        throw new Exception("Attempted to unregister an event that was not present on the target entity system. (Event was not registered, are you using the right generic types?)");
                }
                //Register the system to receieve the event
                EventComponentPair eventComponentPair = new EventComponentPair(typeof(GEvent), typeToRegister);
                lock (RegisteredSystemSignalHandlers)
                {
                    lock (_linkedHandlers)
                    {
                        if (!_linkedHandlers.ContainsKey((typeToRegister, eventHandler)))
                        {
                            throw new Exception($"Attempting to unregister an event handler that isn't registered on {GetType()}.");
                        }
                        //Create and return an event handler so that it can be 
                        RegisteredSystemSignalHandlers[eventComponentPair].Remove(_linkedHandlers[(typeToRegister, eventHandler)]);
                        if (RegisteredSystemSignalHandlers[eventComponentPair].Count == 0)
                        {
                            RegisteredSystemSignalHandlers.Remove(eventComponentPair);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Queues an action for execution, which will be executed on the correct thread for this system.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="callingFile"></param>
        /// <param name="callingMember"></param>
        /// <param name="callingLine"></param>
        public void QueueAction(Action action,
            [CallerFilePath] string callingFile = "",
            [CallerMemberName] string callingMember = "",
            [CallerLineNumber] int callingLine = 0)
        {
            lock (RegisteredSystemSignalHandlers)
            {
                invokationQueue.Enqueue(new InvokationAction(action, callingFile, callingMember, callingLine));
                if (isWaiting)
                    waitHandle.Set();
            }
        }

    }
}

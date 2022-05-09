using CorgEng.Core;
using CorgEng.Core.Dependencies;
using CorgEng.Core.Modules;
using CorgEng.EntityComponentSystem.Components;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.GenericInterfaces.ContentLoading;
using CorgEng.GenericInterfaces.Logging;
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
            public override bool SetProperty(string name, IPropertyDef property)
            {
                return false;
            }
        }

        [UsingDependency]
        private static ILogger Logger;

        internal delegate void SystemEventHandlerDelegate(Entity entity, Component component, Event signal);

        /// <summary>
        /// Matches event and component types to registered signal handlers on systems
        /// </summary>
        internal static Dictionary<EventComponentPair, List<SystemEventHandlerDelegate>> RegisteredSystemSignalHandlers { get; } = new Dictionary<EventComponentPair, List<SystemEventHandlerDelegate>>();

        /// <summary>
        /// Wait handler. This will cause the thread to pause until an event that needs handling is raised.
        /// </summary>
        private readonly AutoResetEvent waitHandle = new AutoResetEvent(false);

        private volatile bool isWaiting = false;

        /// <summary>
        /// The invokation queue. A queue of actions that need to be triggered (Raised events)
        /// </summary>
        private readonly ConcurrentQueue<Action> invokationQueue = new ConcurrentQueue<Action>();

        /// <summary>
        /// A static list of all entity systems in use
        /// </summary>
        private static List<EntitySystem> EntitySystems = new List<EntitySystem>();

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
            IEnumerable<Type> locatedSystems = AppDomain.CurrentDomain.GetAssemblies()
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
        private void SystemThread()
        {
            while (!CorgEngMain.Terminated)
            {
                //Wait until we are awoken again
                if (invokationQueue.Count == 0)
                {
                    isWaiting = true;
                    waitHandle.WaitOne();
                    isWaiting = false;
                }
                Action firstInvokation;
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
        /// Registers a signal to a global event.
        /// </summary>
        /// <typeparam name="GEvent">The type of the global event to subscribe to.</typeparam>
        /// <param name="eventHandler">The method to be invoked when the global event is raised.</param>
        public void RegisterGlobalEvent<GEvent>(Action<GEvent> eventHandler)
            where GEvent : Event
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
                RegisteredSystemSignalHandlers[eventComponentPair].Add((Entity entity, Component component, Event signal) =>
                {
                    invokationQueue.Enqueue(() =>
                    {
                        eventHandler((GEvent)signal);
                    });
                    if(isWaiting)
                        waitHandle.Set();
                });
            }
        }

        /// <summary>
        /// Register to a local event
        /// </summary>
        public void RegisterLocalEvent<GComponent, GEvent>(Action<Entity, GComponent, GEvent> eventHandler)
            where GComponent : Component
            where GEvent : Event
        {
            //Register the component to recieve the target event on the event manager
            lock (EventManager.RegisteredEvents)
            {
                if (!EventManager.RegisteredEvents.ContainsKey(typeof(GComponent)))
                    EventManager.RegisteredEvents.Add(typeof(GComponent), new List<Type>());
                if (!EventManager.RegisteredEvents[typeof(GComponent)].Contains(typeof(GEvent)))
                    EventManager.RegisteredEvents[typeof(GComponent)].Add(typeof(GEvent));
            }
            //Register the system to receieve the event
            EventComponentPair eventComponentPair = new EventComponentPair(typeof(GEvent), typeof(GComponent));
            lock (RegisteredSystemSignalHandlers)
            {
                if (!RegisteredSystemSignalHandlers.ContainsKey(eventComponentPair))
                    RegisteredSystemSignalHandlers.Add(eventComponentPair, new List<SystemEventHandlerDelegate>());
                RegisteredSystemSignalHandlers[eventComponentPair].Add((Entity entity, Component component, Event signal) =>
                {
                    invokationQueue.Enqueue(() =>
                    {
                        eventHandler(entity, (GComponent)component, (GEvent)signal);
                    });
                    if(isWaiting)
                        waitHandle.Set();
                });
            }
        }

    }
}

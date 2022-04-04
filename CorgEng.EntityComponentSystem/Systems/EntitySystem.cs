using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Components;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.GenericInterfaces.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace CorgEng.EntityComponentSystem.Systems
{
    public abstract class EntitySystem
    {

        /// <summary>
        /// Internal global event component specifically for handling global signals
        /// </summary>
        internal class GlobalEventComponent : Component { }

        [UsingDependency]
        private static ILogger Logger;

        internal delegate void SystemEventHandlerDelegate(Entity entity, Component component, Event signal);

        /// <summary>
        /// Matches event and component types to registered signal handlers on systems
        /// </summary>
        internal static Dictionary<EventComponentPair, List<SystemEventHandlerDelegate>> RegisteredSystemSignalHandlers { get; } = new Dictionary<EventComponentPair, List<SystemEventHandlerDelegate>>();

        private readonly AutoResetEvent waitHandle = new AutoResetEvent(false);

        private readonly ConcurrentQueue<Action> invokationQueue = new ConcurrentQueue<Action>();

        public EntitySystem()
        {
            Thread thread = new Thread(SystemThread);
            thread.Name = $"{this} thread";
            thread.Start();
        }

        public abstract void SystemSetup();

        private void SystemThread()
        {
            //TODO: Make this run until the game is closed
            while (true)
            {
                //Wait until we are awoken again
                if (invokationQueue.Count == 0)
                    waitHandle.WaitOne();
                Action firstInvokation;
                invokationQueue.TryDequeue(out firstInvokation);
                if (firstInvokation != null)
                {
                    try
                    {
                        //Invoke the provided action
                        firstInvokation.Invoke();
                    }
                    catch (Exception e)
                    {
                        Logger.WriteLine(e);
                    }
                }
            }
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
            if (!EventManager.RegisteredEvents.ContainsKey(typeof(GlobalEventComponent)))
                EventManager.RegisteredEvents.Add(typeof(GlobalEventComponent), new List<Type>());
            if (!EventManager.RegisteredEvents[typeof(GlobalEventComponent)].Contains(typeof(GEvent)))
                EventManager.RegisteredEvents[typeof(GlobalEventComponent)].Add(typeof(GEvent));
            //Register the system to receieve the event
            EventComponentPair eventComponentPair = new EventComponentPair(typeof(GEvent), typeof(GlobalEventComponent));
            if (!RegisteredSystemSignalHandlers.ContainsKey(eventComponentPair))
                RegisteredSystemSignalHandlers.Add(eventComponentPair, new List<SystemEventHandlerDelegate>());
            RegisteredSystemSignalHandlers[eventComponentPair].Add((Entity entity, Component component, Event signal) => {
                invokationQueue.Enqueue(() => {
                    eventHandler.Invoke((GEvent)signal);
                });
                waitHandle.Set();
            });
        }

        /// <summary>
        /// Register to a local event
        /// </summary>
        public void RegisterLocalEvent<GComponent, GEvent>(Action<Entity, GComponent, GEvent> eventHandler)
            where GComponent : Component
            where GEvent : Event
        {
            //Register the component to recieve the target event on the event manager
            if (!EventManager.RegisteredEvents.ContainsKey(typeof(GComponent)))
                EventManager.RegisteredEvents.Add(typeof(GComponent), new List<Type>());
            if(!EventManager.RegisteredEvents[typeof(GComponent)].Contains(typeof(GEvent)))
                EventManager.RegisteredEvents[typeof(GComponent)].Add(typeof(GEvent));
            //Register the system to receieve the event
            EventComponentPair eventComponentPair = new EventComponentPair(typeof(GEvent), typeof(GComponent));
            if (!RegisteredSystemSignalHandlers.ContainsKey(eventComponentPair))
                RegisteredSystemSignalHandlers.Add(eventComponentPair, new List<SystemEventHandlerDelegate>());
            RegisteredSystemSignalHandlers[eventComponentPair].Add((Entity entity, Component component, Event signal) => {
                invokationQueue.Enqueue(() => {
                    eventHandler.Invoke(entity, (GComponent)component, (GEvent)signal);
                });
                waitHandle.Set();
            });
        }

    }
}

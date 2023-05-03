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
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using static CorgEng.GenericInterfaces.EntityComponentSystem.IEntitySystemManager;

namespace CorgEng.EntityComponentSystem.Systems
{
    public abstract class EntitySystem
    {

        public static int EntitySystemCount = 0;

        private string name;

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

        /// <summary>
        /// The invokation queue. A queue of actions that need to be triggered (Raised events)
        /// </summary>
        protected readonly ConcurrentQueue<InvokationAction> invokationQueue = new ConcurrentQueue<InvokationAction>();

        /// <summary>
        /// The flags of this system
        /// </summary>
        public abstract EntitySystemFlags SystemFlags { get; }

        /// <summary>
        /// If we have been targeted for a kill
        /// </summary>
        protected bool assassinated = false;

        /// <summary>
        /// The world that we belong to
        /// </summary>
        protected IWorld world;

        /// <summary>
        /// The thread manager flags.
        /// </summary>
        internal ThreadManagerFlags threadManagerFlags;

        private EntitySystemThreadManager threadManager;

        public EntitySystem()
        {
            name = $"{GetType().Name}-{Interlocked.Increment(ref EntitySystemCount)}";
        }

        public void JoinWorld(IWorld world)
        {
            this.world = world;
            //Register the global signal to handle closing the game
            RegisterGlobalEvent((GameClosedEvent e) => { });
        }

        public void JoinEntitySystemManager(EntitySystemThreadManager threadManager)
        {
            this.threadManager = threadManager;
            // Queue the system for a single fire, in case it is a processing system
            QueueProcessing();
        }

        public abstract void SystemSetup(IWorld world);

        /// <summary>
        /// Amount of runs we should perform before stopping
        /// </summary>
        private int tickRunsRemaining = 0;

        /// <summary>
        /// The system thread. Waits until an invokation is required and then triggers it
        /// on the system's thread.
        /// Returns true if the run was completed, returns false if the system should be
        /// requeued for further processing.
        /// </summary>
        public virtual bool PerformRun(EntitySystemThreadManager threadManager)
        {
            // Refresh the tick runs that we need to
            if (tickRunsRemaining == 0)
            {
                tickRunsRemaining = invokationQueue.Count;
            }
            while (invokationQueue.Count > 0 && tickRunsRemaining-- > 0)
            {
                InvokationAction firstInvokation;
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
                // Check to see if someone else is doing something more important than us
                if (CheckRelinquishControl())
                {
                    return false;
                }
            }
            return invokationQueue.IsEmpty;
        }

        private object _controlLockObject = new object();

        /// <summary>
        /// Check if we should relinquish control of this system to another source.
        /// This will happen if we are a low priority processing thread and a high priority
        /// lock has been requested.
        /// </summary>
        /// <returns></returns>
        internal bool CheckRelinquishControl()
        {
            if ((threadManagerFlags & ThreadManagerFlags.REQUESTED_HIGH) == ThreadManagerFlags.REQUESTED_HIGH)
            {
                // Release our thread manager lock
                lock (this)
                {
                    threadManagerFlags &= ~ThreadManagerFlags.LOCKED_LOW;
                    lock (_controlLockObject)
                    {
                        Monitor.PulseAll(_controlLockObject);
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Try to acquire a low priority lock for this processing system.
        /// Returns false if we were unable to acquire the lock due to this system
        /// already processing.
        /// </summary>
        /// <returns></returns>
        internal bool TryAcquireLowPriorityLock()
        {
            lock (this)
            {
                if ((threadManagerFlags & (ThreadManagerFlags.LOCKED_HIGH | ThreadManagerFlags.LOCKED_LOW | ThreadManagerFlags.REQUESTED_HIGH)) != 0)
                    return false;
                threadManagerFlags |= ThreadManagerFlags.LOCKED_LOW;
                return true;
            }
        }

        /// <summary>
        /// Release the lock currently held by this thread.
        /// Ownership of the lock is not enforced, I trust the internal
        /// code to actually have ownership of a lock.
        /// </summary>
        public void ReleaseLock()
        {
            lock (this)
            {
                threadManagerFlags &= ~ThreadManagerFlags.LOCKED_LOW;
                threadManagerFlags &= ~ThreadManagerFlags.LOCKED_HIGH;
                // If we aren't requested, then add ourselves to the processing queue
                if ((threadManagerFlags & (ThreadManagerFlags.REQUESTED_HIGH)) == 0)
                {
                    QueueProcessing();
                }
            }
            lock (_controlLockObject)
            {
                Monitor.PulseAll(_controlLockObject);
            }
        }

        internal void ReleaseInternalLock()
        {
            lock (this)
            {
                threadManagerFlags &= ~ThreadManagerFlags.LOCKED_LOW;
            }
            lock (_controlLockObject)
            {
                Monitor.PulseAll(_controlLockObject);
            }
        }

        /// <summary>
        /// Acquire a high priority lock. This action is blocking
        /// until the lock is acquired.
        /// Always returns true.
        /// </summary>
        public bool AcquireHighPriorityLock()
        {
            // Request access control
            bool isControlled = false;
            lock (this)
            {
                isControlled = (threadManagerFlags & (ThreadManagerFlags.LOCKED_LOW | ThreadManagerFlags.LOCKED_HIGH)) != 0;
                if (isControlled)
                    threadManagerFlags |= ThreadManagerFlags.REQUESTED_HIGH;
                else
                    threadManagerFlags |= ThreadManagerFlags.LOCKED_HIGH;
            }
            while (isControlled)
            {
                // Wait until the thread manager flags are changed
                lock (_controlLockObject)
                {
                    // Add the flag to indicate that we want to claim the system
                    lock (this)
                    {
                        // Check to see if we are controlled
                        isControlled = (threadManagerFlags & (ThreadManagerFlags.LOCKED_LOW | ThreadManagerFlags.LOCKED_HIGH)) != 0;
                        if (!isControlled)
                        {
                            // Claim the system and release our request
                            threadManagerFlags |= ThreadManagerFlags.LOCKED_HIGH;
                            threadManagerFlags &= ~ThreadManagerFlags.REQUESTED_HIGH;
                            return true;
                        }
                        // Request high priority
                        threadManagerFlags |= ThreadManagerFlags.REQUESTED_HIGH;
                    }
                    Monitor.Wait(_controlLockObject);
                }

            }
            // We now have a high priority lock over this system
            return true;
        }

        /// <summary>
        /// Queue the entity system to process if it isn't already queued to process.
        /// </summary>
        internal void QueueProcessing()
        {
            lock (this)
            {
                // If the system is not queued, queue it.
                if ((threadManagerFlags & ThreadManagerFlags.QUEUED_SYSTEM) != 0)
                    return;
                threadManagerFlags |= ThreadManagerFlags.QUEUED_SYSTEM;
                threadManager.EnqueueSystemForProcessing(this);
            }
        }

        /// <summary>
        /// Kills this and only this system
        /// </summary>
        public void Kill()
        {
            //Effective
            assassinated = true;
            //Tell the system to process its death
            QueueProcessing();
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
            world.EntitySystemManager.RegisterEventType(typeof(GlobalEventComponent), typeof(GEvent));
            //Register the system to receieve the event
            EventComponentPair eventComponentPair = new EventComponentPair(typeof(GEvent), typeof(GlobalEventComponent));
            // Everything inside this is an extremely hot path since its the game engine's function call.
            // This should be pretty cheap (although it probably isn't very cheap right now).
            SystemEventHandlerDelegate createdEventHandler = (IEntity entity, IComponent component, IEvent signal, bool synchronous, string file, string member, int lineNumber) =>
            {
                //Check to see if we were processing when the event was fired
                if (NetworkConfig != null
                    && NetworkConfig.NetworkingActive
                    && ((SystemFlags & EntitySystemFlags.HOST_SYSTEM) == 0 || !NetworkConfig.ProcessServerSystems)
                    && ((SystemFlags & EntitySystemFlags.CLIENT_SYSTEM) == 0 || !NetworkConfig.ProcessClientSystems))
                    return;
                // Attempt to gain ownership of the system
                // There is a potential issue here: Synchronous event deadlock.
                if ((synchronous && AcquireHighPriorityLock()) || ((SystemFlags & EntitySystemFlags.NO_IMMEDIATE_ASYNCHRONOUS_EVENTS) == 0 && TryAcquireLowPriorityLock()))
                {
                    // Fire the event on the current thread
                    try
                    {
                        // Now that the current thread has the processing key, we can immediately invoke the requested action
                        eventHandler.Invoke((GEvent)signal);
                    }
                    catch (Exception e)
                    {
                        Logger?.WriteLine($"Event Called (and fired immediately) from: {Environment.StackTrace}\n{e}", LogType.ERROR);
                    }
                    finally
                    {
                        // Ensure that we definitely release this lock
                        ReleaseLock();
                    }
                }
                else
                {
                    // If this is not a synchronous event, just queue the event
                    ConcurrentQueue<InvokationAction> targetQueue = invokationQueue;
                    targetQueue.Enqueue(new InvokationAction(() =>
                    {
                        eventHandler.Invoke((GEvent)signal);
                    }, file, member, lineNumber));
                    //Wake up the system if its sleeping
                    QueueProcessing();
                }
            };
            lock (_globalLinkedHandlers)
            {
                if (_globalLinkedHandlers.ContainsKey(eventHandler))
                    throw new Exception("Attempting to register a global signal handler that is already registered to this system.");
                _globalLinkedHandlers.Add(eventHandler, createdEventHandler);
            }
            world.EntitySystemManager.RegisterSystemEventHandler(eventComponentPair, createdEventHandler);
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
            world.EntitySystemManager.UnregisterEventType(typeof(GlobalEventComponent), typeof(GEvent));
            //Register the system to receieve the event
            EventComponentPair eventComponentPair = new EventComponentPair(typeof(GEvent), typeof(GlobalEventComponent));
            //Hnadle unregistering
            lock (_globalLinkedHandlers)
            {
                if (!_globalLinkedHandlers.ContainsKey(eventHandler))
                    throw new Exception("Attempting to unregister a global event handler that isn't currently registered.");
                world.EntitySystemManager.UnregisterSystemEventHandler(eventComponentPair, _globalLinkedHandlers[eventHandler]);
                _globalLinkedHandlers.Remove(eventHandler);
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
                world.EntitySystemManager.RegisterEventType(typeToRegister, typeof(GEvent));
                //Register the system to receieve the event
                EventComponentPair eventComponentPair = new EventComponentPair(typeof(GEvent), typeToRegister);
                //Create and return an event handler so that it can be 
                createdEventHandler = (IEntity entity, IComponent component, IEvent signal, bool synchronous, string callingFile, string callingMember, int callingLine) =>
                {
                    //Check to see if we were processing when the event was fired
                    if (NetworkConfig != null
                        && NetworkConfig.NetworkingActive
                        && ((SystemFlags & EntitySystemFlags.HOST_SYSTEM) == 0 || !NetworkConfig.ProcessServerSystems)
                        && ((SystemFlags & EntitySystemFlags.CLIENT_SYSTEM) == 0 || !NetworkConfig.ProcessClientSystems))
                        return;
                    // Attempt to gain ownership of the system
                    // There is a potential issue here: Synchronous event deadlock.
                    if ((synchronous && AcquireHighPriorityLock()) || ((SystemFlags & EntitySystemFlags.NO_IMMEDIATE_ASYNCHRONOUS_EVENTS) == 0 && TryAcquireLowPriorityLock()))
                    {
                        // Fire the event on the current thread
                        try
                        {
                            // Now that the current thread has the processing key, we can immediately invoke the requested action
                            eventHandler.Invoke(entity, (GComponent)component, (GEvent)signal);
                        }
                        catch (Exception e)
                        {
                            Logger?.WriteLine($"Event Called (and fired immediately) from: {Environment.StackTrace}\n{e}", LogType.ERROR);
                        }
                        finally
                        {
                            // Ensure that we definitely release this lock
                            ReleaseLock();
                        }
                    }
                    else
                    {
                        // If this is not a synchronous event, just queue the event
                        ConcurrentQueue<InvokationAction> targetQueue = invokationQueue;
                        targetQueue.Enqueue(new InvokationAction(() =>
                        {
                            eventHandler.Invoke(entity, (GComponent)component, (GEvent)signal);
                        }, callingFile, callingMember, callingLine));
                        // Wake up the system if its sleeping
                        QueueProcessing();
                    }
                };
                lock (_linkedHandlers)
                {
                    if (_linkedHandlers.ContainsKey((typeToRegister, eventHandler)))
                        throw new Exception("Attempting to register an event that is already registered.");
                    _linkedHandlers.Add((typeToRegister, eventHandler), createdEventHandler);
                }
                world.EntitySystemManager.RegisterSystemEventHandler(eventComponentPair, createdEventHandler);
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
                world.EntitySystemManager.UnregisterEventType(typeToRegister, typeof(GEvent));
                //Register the system to receieve the event
                EventComponentPair eventComponentPair = new EventComponentPair(typeof(GEvent), typeToRegister);
                lock (_linkedHandlers)
                {
                    if (!_linkedHandlers.ContainsKey((typeToRegister, eventHandler)))
                    {
                        throw new Exception($"Attempting to unregister an event handler that isn't registered on {GetType()}.");
                    }
                    world.EntitySystemManager.UnregisterSystemEventHandler(eventComponentPair, _linkedHandlers[(typeToRegister, eventHandler)]);
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
            invokationQueue.Enqueue(new InvokationAction(action, callingFile, callingMember, callingLine));
            QueueProcessing();
        }

        public override string ToString()
        {
            return name;
        }
    }
}

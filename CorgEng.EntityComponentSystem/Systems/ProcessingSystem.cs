//#define PROCESSING_SYSTEM_DEBUG

using CorgEng.Core;
using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.EntityComponentSystem.Implementations.Deletion;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Systems
{
    public abstract class ProcessingSystem : EntitySystem
    {

        [UsingDependency]
        private static ILogger Logger;

        /// <summary>
        /// A thread safe dictionary containing everything that is currently processing
        /// </summary>
        private volatile ConcurrentDictionary<IEntity, Action<double>> processingQueue = new ConcurrentDictionary<IEntity, Action<double>>();

        /// <summary>
        /// How often do we process
        /// </summary>
        protected abstract int ProcessDelay { get; }

        /// <summary>
        /// The time that this system last fired processes.
        /// Used to calculate delta time.
        /// </summary>
        private double lastFireTime;

        /// <summary>
        /// The current delta time of this system.
        /// Will be the same as lastFireTime, unless this system is falling behind on processing
        /// </summary>
        private double deltaTime;

        //The current enumerator of things that need processing
        private IEnumerator<Action<double>> currentRun;

        //Did we continue
        private bool continued = false;

        /// <summary>
        /// Multiplier for delta time, allows to slow or speed up processing
        /// events, for example if you have time control elements.
        /// </summary>
        public static double DeltaTimeMultiplier { get; set; } = 1;

        private int tickRunsRemaining = 0;
        private bool isRunningProcesses = false;

        private double nextProcessTime;

        /// <summary>
        /// Override initial behaviour to also be able to handle processing at regular intervals.
        /// This works slightly differently as it needs to allow for adequate time to process
        /// the things it needs to process while still letting signals pass.
        /// </summary>
        public override bool PerformRun(EntitySystemThreadManager threadManager)
        {
            if (!isRunningProcesses)
            {
                // Refresh the tick runs that we need to
                if (tickRunsRemaining <= 0)
                {
                    tickRunsRemaining = invokationQueue.Count;
                }
                // Run the processes that we need to
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
                // Move to the next step (processes)
                isRunningProcesses = true;
            }
            if (isRunningProcesses)
            {
                // We are not ready to re-fire at this time
                if (!continued && nextProcessTime > CorgEngMain.Time)
                {
                    isRunningProcesses = false;
#if PROCESSING_SYSTEM_DEBUG
                    Logger.WriteLine($"{this} is not ready to fire, fire at: {nextProcessTime}, current time: {CorgEngMain.Time}", LogType.DEBUG);
                    if (calledFromProcess)
                    {
                        Logger.WriteLine($"wtf, we were claled from runningProcess::: {this}", LogType.TEMP);
                    }
#endif
                    return invokationQueue.IsEmpty;
                }
                //If we aren't continued, fetch the new queue
                if (!continued)
                {
                    currentRun = processingQueue.Values.GetEnumerator();
                    //Calculate delta time
                    deltaTime = CorgEngMain.Time - lastFireTime;
                    //Mark the last fire time as now
                    lastFireTime = CorgEngMain.Time;
#if PROCESSING_SYSTEM_DEBUG
                    Logger.WriteLine($"Firing {ToString()} processing system at {CorgEngMain.Time} (Delay: {ProcessDelay}ms)", LogType.DEBUG);
#endif
                }
                //Mark the system as being completed, unless we have to break out early
                continued = false;
                while (currentRun.MoveNext())
                {
                    //Nothing in this list
                    if (currentRun.Current == null)
                    {
                        break;
                    }
                    //Do the process
                    try
                    {
                        currentRun.Current(deltaTime * DeltaTimeMultiplier);
                    }
                    catch (Exception e)
                    {
                        Logger.WriteLine(e, LogType.ERROR);
                    }
                    // Check to see if someone else is doing something more important than us
                    if (CheckRelinquishControl())
                    {
                        continued = true;
                        return false;
                    }
                }
                // Move to the first step again
                isRunningProcesses = false;
                //Calculate how long we have to wait for, based on the last fire time, our current time
                //and the system wait time.
                //Stay in sync! if this value is less than 0, we are overtiming.
                int waitTime = (int)Math.Max(1000 * (lastFireTime - CorgEngMain.Time) + ProcessDelay, 0);
                nextProcessTime = CorgEngMain.Time + (waitTime * 0.001);
#if PROCESSING_SYSTEM_DEBUG
                Logger.WriteLine($"{ToString()} queued to fire in {waitTime}ms. It should fire at {nextProcessTime}, it is currently {CorgEngMain.Time}", LogType.DEBUG);
#endif
                //If we recieve a signal, loop straight round to processing signal handlers.
                //Otherwise, we reached the next processor step, so do processing
                if (waitTime > 0)
                {
                    threadManager.FireSystemIn(this, waitTime);
                }
                // We need to immediately re-fire to process the next tick
                else
                    return false;
            }
            return invokationQueue.IsEmpty;
        }

        private HashSet<Type> registeredDeletionHandlers = new HashSet<Type>();

        /// <summary>
        /// Start processing an event.
        /// This will trigger regular updates at the process interval of this subsystem.
        /// </summary>
        /// <typeparam name="GComponent"></typeparam>
        /// <param name="target"></param>
        /// <param name="onProcessTask"></param>
        public void RegisterProcess<GComponent>(IEntity target, Action<IEntity, GComponent, double> onProcessTask)
            where GComponent : IComponent
        {
            //How does this actually work, targetComponent gets saved in memory or becomes a local inside of the lambda function?
            //Locate the component to fetch for processing (We do fetching here, since it saves time)
            GComponent targetComponent = target.GetComponent<GComponent>();
            //Perform the process
            if (!processingQueue.TryAdd(target, (deltaTime) =>
                {
                    onProcessTask(target, targetComponent, deltaTime);
                }))
            {
                throw new Exception("Attempted to begin processing an entity that is already processing on this system. This is not allowed.");
            }
            //Check for adding the deletion handler
            if (!registeredDeletionHandlers.Contains(typeof(GComponent)))
            {
                //We need to know when the component is removed from an entity
                RegisterLocalEvent<GComponent, ComponentRemovedEvent>((entity, component, signal) => {
                    if (signal.Component.Equals(component))
                    {
                        processingQueue.TryRemove(entity, out _);
                    }
                });
            }
        }

        /// <summary>
        /// Stop processing a specific entity
        /// </summary>
        /// <param name="target"></param>
        public void StopProcesing(IEntity target)
        {
            processingQueue.TryRemove(target, out _);
        }

    }
}

using CorgEng.Core;
using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        /// Override initial behaviour to also be able to handle processing at regular intervals
        /// </summary>
        protected override void SystemThread()
        {
            while (!CorgEngMain.Terminated && !assassinated)
            {
                //If we aren't continued, fetch the new queue
                if (!continued)
                {
                    currentRun = processingQueue.Values.GetEnumerator();
                    //Calculate delta time
                    deltaTime = CorgEngMain.Time - lastFireTime;
                    //Mark the last fire time as now
                    lastFireTime = CorgEngMain.Time;
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
                        currentRun.Current(deltaTime);
                    }
                    catch (Exception e)
                    {
                        Logger.WriteLine(e, LogType.ERROR);
                    }
                    //Check to see if a new signal that needs to be handled has come in
                    if (invokationQueue.Count > 0)
                    {
                        //We have an invokation to handle, we will continue processing on the next cycle round
                        continued = true;
                        break;
                    }
                }
                //Allocate time to processing signal handles
                Action current;
                while (invokationQueue.TryDequeue(out current))
                {
                    try
                    {
                        current();
                    }
                    catch (Exception e)
                    {
                        Logger?.WriteLine(e, LogType.ERROR);
                    }
                }
                //If we completed processing, wait for the specified time, or until another signal handler comes in
                //If a signal comes in when we loop round, we will go straight back to handling signals again
                if (!continued)
                {
                    isWaiting = true;
                    //Calculate how long we have to wait for, based on the last fire time, our current time
                    //and the system wait time.
                    //Stay in sync! if this value is less than 0, we are overtiming.
                    int waitTime = (int)Math.Max(1000 * (lastFireTime - CorgEngMain.Time) + ProcessDelay, 0);
                    //If we recieve a signal, loop straight round to processing signal handlers.
                    //Otherwise, we reached the next processor step, so do processing
                    if(waitTime > 0)
                        continued = waitHandle.WaitOne(waitTime);
                    else
                        continued = false;
                    isWaiting = false;
                }
            }
            //Terminated
            Logger?.WriteLine($"Terminated EntitySystem thread: {this}", LogType.LOG);
        }

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
            processingQueue.TryAdd(target, (deltaTime) => {
                onProcessTask(target, targetComponent, deltaTime);
            });
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

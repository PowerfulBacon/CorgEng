using CorgEng.Core;
using CorgEng.Core.Dependencies;
using CorgEng.Core.Modules;
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
    public enum ThreadManagerFlags
    {
        QUEUED_SYSTEM = 1 << 0,
        LOCKED_LOW = 1 << 1,
        REQUESTED_HIGH = 1 << 2,
        LOCKED_HIGH = 1 << 3,
    }

    public class EntitySystemThreadManager
    {

        private static ConcurrentBag<EntitySystemThreadManager> activeBags = new ConcurrentBag<EntitySystemThreadManager>();

        [UsingDependency]
        private static ILogger Logger;

        private EntitySystemThread[] runningWorkers;

        internal bool working = true;

        internal ConcurrentQueue<EntitySystem> queuedSystems = new ConcurrentQueue<EntitySystem>();

        internal ConcurrentQueue<EntitySystemThread> sleepingThreads = new ConcurrentQueue<EntitySystemThread>();

        public EntitySystemThreadManager(int workerThreads)
        {
            if (workerThreads < 1)
            {
                workerThreads = 1;
            }
            List<string> threadNumbers = new List<string>();
            runningWorkers = new EntitySystemThread[workerThreads];
            for (int i = 0; i < workerThreads; i++)
            {
                runningWorkers[i] = new EntitySystemThread(this, i);
                threadNumbers.Add("Thread: " + runningWorkers[i].thread.ManagedThreadId);
            }
            Logger.WriteLine($"Entity system thread manager created with {workerThreads} maximum threads. Thread IDs: {string.Join(", ", threadNumbers)}", LogType.DEBUG);
            activeBags.Add(this);
        }

        [ModuleTerminate]
        public static void ShutdownEntitySystemManagers()
        {
            lock (activeBags)
            {
                foreach (EntitySystemThreadManager thread in activeBags)
                {
                    thread.working = false;
                }
                activeBags.Clear();
            }
        }

        public void Cleanup()
        {
            lock (activeBags)
            {
                working = false;
            }
        }

        /// <summary>
        /// Enqueue a system for processing.
        /// </summary>
        /// <param name="system"></param>
        public void EnqueueSystemForProcessing(EntitySystem system)
        {
            queuedSystems.Enqueue(system);
            // Wake up a single thread to handle this
            if (sleepingThreads.TryDequeue(out EntitySystemThread result))
            {
                result.WakeUp();
            }
        }

        public void FireSystemIn(EntitySystem system, double fireTime)
        {
            CorgEngMain.ExecuteIn(() => {
                Logger.WriteLine($"Attempting to queue system {system} for processing fire at {CorgEngMain.Time}", LogType.TEMP);
                system.QueueProcessing();
            }, fireTime);
        }

    }

    public class EntitySystemThread
    {

        [UsingDependency]
        private static ILogger Logger;

        public int identifier;

        private bool isRunning = false;

        private EntitySystemThreadManager threadManager;

        internal Thread thread;

        public EntitySystemThread(EntitySystemThreadManager threadManager, int identifier)
        {
            this.identifier = identifier;
            this.threadManager = threadManager;
            thread = new Thread(WorkerThread);
            thread.Start();
            WakeUp();
        }

        /// <summary>
        /// Attempt to wakeup the entity system thread.
        /// </summary>
        public void WakeUp()
        {
            lock (this)
            {
                if (isRunning)
                    return;
                Monitor.Pulse(this);
                isRunning = true;
                //Logger.WriteLine($"Entity System Thread {identifier} waking up...", LogType.DEBUG);
            }
        }

        /// <summary>
        /// The entity system worker thread.
        /// Will process any subsystems that are not currently
        /// owned by another process.
        /// The thread will terminate once this is completed.
        /// </summary>
        private void WorkerThread()
        {
            try
            {
                while (threadManager.working)
                {
                    // Begin working through the systems
                    while (threadManager.queuedSystems.TryDequeue(out EntitySystem entitySystem))
                    {
                        lock (entitySystem)
                        {
                            entitySystem.threadManagerFlags &= ~ThreadManagerFlags.QUEUED_SYSTEM;
                            if (!entitySystem.TryAcquireLowPriorityLock())
                            {
                                // We will be re-queued when the lock is released.
                                continue;
                            }
                        }
                        try
                        {
                            if (!entitySystem.PerformRun(threadManager))
                            {
                                // Requeue the system for further processing
                                entitySystem.QueueProcessing();
                            }
                        }
                        // Release our low priority lock at the end without side effects.
                        finally
                        {
                            entitySystem.ReleaseInternalLock();
                        }
                    }
                    // Check if we were requested to wakeup while trying to sleep
                    if (threadManager.queuedSystems.IsEmpty)
                    {
                        lock (this)
                        {
                            // We can spin down now
                            isRunning = false;
                            threadManager.sleepingThreads.Enqueue(this);
                            // Wait until we are woken up again
                            //Logger.WriteLine($"Entity System Thread {identifier} entering sleep mode...", LogType.DEBUG);
                            Monitor.Wait(this);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.WriteLine($"FATAL EXCEPTION: THREAD MANAGER CRASHED\n{e}.", LogType.ERROR);
            }
        }

    }
}

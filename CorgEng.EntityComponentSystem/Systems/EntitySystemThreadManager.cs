﻿using System;
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
            runningWorkers = new EntitySystemThread[workerThreads];
            for (int i = 0; i < workerThreads; i++)
            {
                runningWorkers[i] = new EntitySystemThread(this);
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
            Task.Run(async () => {
                await Task.Delay((int)fireTime);
                system.QueueProcessing();
            });
        }

    }

    public class EntitySystemThread
    {

        private bool isRunning = false;

        private EntitySystemThreadManager threadManager;

        private Thread thread;

        public EntitySystemThread(EntitySystemThreadManager threadManager)
        {
            this.threadManager = threadManager;
            thread = new Thread(WorkerThread);
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
                isRunning = true;
                thread = new Thread(WorkerThread);
                thread.Start();
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
            while (threadManager.working)
            {
                // Begin working through the systems
                while (threadManager.queuedSystems.TryDequeue(out EntitySystem entitySystem))
                {
                    entitySystem.threadManagerFlags &= ~ThreadManagerFlags.QUEUED_SYSTEM;
                    if (!entitySystem.PerformRun(threadManager))
                    {
                        // Requeue the system for further processing
                        entitySystem.QueueProcessing();
                    }
                }
                lock (threadManager)
                {
                    // Check if we were requested to wakeup while trying to sleep
                    if (threadManager.queuedSystems.IsEmpty)
                    {
                        // We can spin down now
                        isRunning = false;
                        threadManager.sleepingThreads.Enqueue(this);
                        return;
                    }
                }
            }
        }

    }
}

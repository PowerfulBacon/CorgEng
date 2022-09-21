using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.AiBehaviours;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.UtilityTypes.BinaryLists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.AiBehaviour
{
    public abstract class BehaviourNode : IBehaviourNode
    {

        [UsingDependency]
        private static IBinaryListFactory BinaryListFactory;

        [UsingDependency]
        private static ILogger Logger;

        public abstract Task<bool> CanStart(IBehaviourManager manager);

        protected IBinaryList<IBehaviourNode> Subtasks;

        public abstract BehaviourContinuationMode ContinuationMode { get; }

        public BehaviourNode()
        {
            Subtasks = BinaryListFactory.CreateEmpty<IBehaviourNode>();
        }

        public async Task<bool> Action(IBehaviourManager manager)
        {
            //If we canno start
            if (!await CanStart(manager))
                return false;

            Logger?.WriteLine($"Starting {GetType()}");

            //Preaction completed
            if (!await PreAction(manager))
                return false;

            Logger?.WriteLine($"Waiting for action...");

            //Wait until we have completed our current action
            int delay = 50;
            while ((manager.CurrentAction?.Completed ?? true) == false && manager.CurrentAction.Failed == false)
            {
                await Task.Delay(delay);
                //Increase delay to lower lag potential
                delay = Math.Min(delay * 2, 2000);
            }

            // We failed
            if (manager.CurrentAction?.Failed ?? false)
            {
                Logger?.WriteLine($"Action {manager.CurrentAction.GetType()} failed");
                return false;
            }

            manager.CurrentAction = null;
            Logger?.WriteLine("Action completed");

            //Complete subtasks
            foreach (BehaviourNode childNode in Subtasks)
            {
                //Run the child action
                if (!await childNode.Action(manager))
                {
                    //If the child node should cancel this task on fail, then cancel
                    if (childNode.ContinuationMode == BehaviourContinuationMode.CANCEL_ON_FAIL)
                        return false;
                }
            }

            Logger?.WriteLine($"Children of {GetType()} completed.");

            //Actions completed
            return await PostAction(manager);
        }

        /// <summary>
        /// The action to run before running children nodes
        /// </summary>
        public virtual async Task<bool> PreAction(IBehaviourManager manager) => true;

        /// <summary>
        /// The action to run after running children nodes
        /// </summary>
        public virtual async Task<bool> PostAction(IBehaviourManager manager) => true;

        /// <summary>
        /// Add on a subtask
        /// </summary>
        /// <param name="priority"></param>
        /// <param name="subtask"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void AddSubtask(int priority, IBehaviourNode subtask)
        {
            Subtasks.Add(priority, subtask);
        }
    }
}

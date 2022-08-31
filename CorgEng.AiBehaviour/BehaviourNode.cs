using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.AiBehaviours;
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

            //Preaction completed
            if (!await PreAction(manager))
                return false;

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

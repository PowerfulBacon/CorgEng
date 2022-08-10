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
    internal abstract class BehaviourNode : IBehaviourNode
    {

        [UsingDependency]
        private static IBinaryListFactory BinaryListFactory;

        public abstract bool CanStart();

        protected IBinaryList<BehaviourNode> Subtasks;

        public abstract BehaviourContinuationMode ContinuationMode { get; }

        public BehaviourNode()
        {
            Subtasks = BinaryListFactory.CreateEmpty<BehaviourNode>();
        }

        public async Task<bool> Action()
        {
            //Preaction completed
            if (!PreAction())
                return false;

            //Complete subtasks
            foreach (BehaviourNode childNode in Subtasks)
            {
                //Run the child action
                if (!await childNode.Action())
                {
                    //If the child node should cancel this task on fail, then cancel
                    if (childNode.ContinuationMode == BehaviourContinuationMode.CANCEL_ON_FAIL)
                        return false;
                }
            }

            //Actions completed
            return PostAction();
        }

        /// <summary>
        /// The action to run before running children nodes
        /// </summary>
        public virtual bool PreAction() => true;

        /// <summary>
        /// The action to run after running children nodes
        /// </summary>
        public virtual bool PostAction() => true;

    }
}

#define BEHAVIOUR_DEBUG

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
        protected static ILogger Logger;

        public abstract Task<bool> CanStart(IBehaviourManager manager);

        protected IBinaryList<IBehaviourNode> Subtasks;

        public abstract BehaviourContinuationMode ContinuationMode { get; }

        public BehaviourNode()
        {
            Subtasks = BinaryListFactory.CreateEmpty<IBehaviourNode>();
        }

        public async Task<bool> Action(IBehaviourManager manager)
        {
            return await Action(manager, 0);
        }

        public async Task<bool> Action(IBehaviourManager manager, int depth = 0)
        {

#if BEHAVIOUR_DEBUG
                Logger.WriteLine($"{GetTabs(depth)}Performing action for {GetType()}", LogType.DEBUG);
#endif

            //If we canno start
            if (!await CanStart(manager))
            {
                OnCancel(manager);
#if BEHAVIOUR_DEBUG
                Logger.WriteLine($"{GetTabs(depth)}Could not start {GetType()}", LogType.DEBUG);
#endif
                return false;
            }

            //Preaction completed
            if (!await PreAction(manager))
            {
                OnCancel(manager);
#if BEHAVIOUR_DEBUG
                Logger.WriteLine($"{GetTabs(depth)}Failed preaction on {GetType()}", LogType.DEBUG);
#endif
                return false;
            }

            if (!await AwaitAction(manager, depth))
            {
                return false;
            }

            //Complete subtasks
            foreach (BehaviourNode childNode in Subtasks)
            {
                if (childNode.ContinuationMode == BehaviourContinuationMode.REPEAT_UNTIL_FAIL)
                {
                    //Repeat this action over and over until it fails
                    while (await childNode.Action(manager, depth + 1))
                    {
#if BEHAVIOUR_DEBUG
                        Logger.WriteLine($"{GetTabs(depth)}Retrying action {childNode.GetType()} (We are {GetType()})", LogType.DEBUG);
#endif
                    }
                }
                else
                {
                    //Run the child action
                    if (!await childNode.Action(manager, depth + 1))
                    {
                        //If the child node should cancel this task on fail, then cancel
                        if (childNode.ContinuationMode == BehaviourContinuationMode.CANCEL_ON_FAIL)
                        {
                            OnCancel(manager);
#if BEHAVIOUR_DEBUG
                            Logger.WriteLine($"{GetTabs(depth)}Failed child action {childNode.GetType()} (We are {GetType()})", LogType.DEBUG);
#endif
                            return false;
                        }
                    }
                }
            }
#if BEHAVIOUR_DEBUG
            Logger.WriteLine($"{GetTabs(depth)}Successfully completed action {GetType()}", LogType.DEBUG);
#endif
            //Perform post action thinking
            if (!await PostAction(manager))
            {
                return false;
            }

            //If we have an action, wait for its completion
            return await AwaitAction(manager, depth);
        }

#if BEHAVIOUR_DEBUG
        private string GetTabs(int depth)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < depth; i++)
            {
                builder.Append("\t");
            }
            return builder.ToString();
        }
#endif

        /// <summary>
        /// Await for our current action to have completed processing
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        private async Task<bool> AwaitAction(IBehaviourManager manager, int depth)
        {
            //Wait until we have completed our current action
            int delay = 50;
            while ((manager.CurrentAction?.Completed ?? true) == false && manager.CurrentAction.Failed == false)
            {
                await Task.Delay(delay);
                //Increase delay to lower lag potential
                delay = Math.Min(delay * 2, 1000);
            }

            // We failed
            if (manager.CurrentAction?.Failed ?? false)
            {
                OnCancel(manager);
                //Cancel that action
                manager.CurrentAction = null;
#if BEHAVIOUR_DEBUG
                Logger.WriteLine($"{GetTabs(depth)}Failed current pawn action {GetType()}", LogType.DEBUG);
#endif
                return false;
            }

            //Cancel that actino
            manager.CurrentAction = null;

            return true;
        }

        public virtual void OnCancel(IBehaviourManager manager) { }

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

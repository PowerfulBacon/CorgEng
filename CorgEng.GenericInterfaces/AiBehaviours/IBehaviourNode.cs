using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.AiBehaviours
{
    /// <summary>
    /// Singleton nodes.
    /// The state of these nodes shouldn't change, but should
    /// affect the state of their manager.
    /// </summary>
    public interface IBehaviourNode
    {

        /// <summary>
        /// How should the parent react if this node fails?
        /// </summary>
        BehaviourContinuationMode ContinuationMode { get; }

        /// <summary>
        /// Can this task be started?
        /// If a work target is inaccessible, then the task cannot be started.
        /// </summary>
        /// <returns></returns>
        Task<bool> CanStart(IBehaviourManager manager);

        /// <summary>
        /// Perform this nodes actions, and any subactions
        /// </summary>
        /// <returns>Returns false if the action failed.</returns>
        Task<bool> Action(IBehaviourManager manager);

    }
}

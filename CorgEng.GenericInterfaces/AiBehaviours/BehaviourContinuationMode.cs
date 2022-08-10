using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.AiBehaviours
{
    public enum BehaviourContinuationMode
    {
        /// <summary>
        /// If the task fails, then cancel the parent action.
        /// </summary>
        CANCEL_ON_FAIL,
        /// <summary>
        /// If the task fails, move on to the next action.
        /// </summary>
        CONTINUE_ON_FAIL,
    }
}

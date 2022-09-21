using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.AiBehaviours
{
    public interface IBehaviourAction
    {

        bool Failed { get; }

        bool Completed { get; }

        /// <summary>
        /// Perform the action with respect to time.
        /// </summary>
        /// <param name="behaviourManager"></param>
        /// <param name="deltaTime"></param>
        /// <returns>Returns true if the action was completed, false if it needs to continue</returns>
        void PerformAction(IBehaviourManager behaviourManager, double deltaTime);

    }
}

using CorgEng.GenericInterfaces.AiBehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.AiBehaviour
{
    internal class BehaviourRoot : BehaviourNode
    {

        public override BehaviourContinuationMode ContinuationMode => BehaviourContinuationMode.CANCEL_ON_FAIL;

        public override Task<bool> CanStart(IBehaviourManager manager)
        {
            return Task.FromResult(true);
        }

    }
}

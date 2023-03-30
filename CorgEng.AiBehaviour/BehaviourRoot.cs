using CorgEng.GenericInterfaces.AiBehaviours;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.AiBehaviour
{
    internal class BehaviourRoot : BehaviourNode
    {
        public BehaviourRoot(IWorld world) : base(world)
        {
        }

        public override BehaviourContinuationMode ContinuationMode => BehaviourContinuationMode.CANCEL_ON_FAIL;

        public override Task<bool> CanStart(IBehaviourManager manager)
        {
            return Task.FromResult(true);
        }

    }
}

using CorgEng.GenericInterfaces.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.AiBehaviours
{
    public interface IBehaviourManagerFactory
    {

        /// <summary>
        /// Create a new behaviour manager attached to the specified pawn.
        /// </summary>
        /// <param name="attachedPawn"></param>
        /// <returns></returns>
        IBehaviourManager CreateBehaviourManager(IEntity attachedPawn, params IBehaviourNode[] nodes);

    }
}

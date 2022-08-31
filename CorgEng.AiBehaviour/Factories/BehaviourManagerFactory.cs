using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.AiBehaviours;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.AiBehaviour.Factories
{
    [Dependency]
    internal class BehaviourManagerFactory : IBehaviourManagerFactory
    {

        public IBehaviourManager CreateBehaviourManager(IEntity attachedPawn, params IBehaviourNode[] behaviourNodes)
        {
            BehaviourManager createdManager = new BehaviourManager(attachedPawn);

            int i = 0;
            foreach (IBehaviourNode behaviourNode in behaviourNodes)
            {
                createdManager.root.AddSubtask(i, behaviourNode);
            }

            return createdManager;
        }

    }
}

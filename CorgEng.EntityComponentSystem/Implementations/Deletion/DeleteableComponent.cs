using CorgEng.EntityComponentSystem.Components;
using CorgEng.GenericInterfaces.ContentLoading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Implementations.Deletion
{
    public class DeleteableComponent : Component
    {
        /// <summary>
        /// Do not sync this component, as it is a default component to all entities.
        /// </summary>
        public override bool IsSynced => false;

    }
}

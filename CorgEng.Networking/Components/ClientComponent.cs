using CorgEng.EntityComponentSystem.Components;
using CorgEng.GenericInterfaces.ContentLoading;
using CorgEng.GenericInterfaces.Networking.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Networking.Components
{
    public class ClientComponent : Component
    {

        /// <summary>
        /// The client attached to this component.
        /// </summary>
        public IClient AttachedClient { get; set; }

        public override bool SetProperty(string name, IPropertyDef property)
        {
            return false;
        }

    }
}

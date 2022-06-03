using CorgEng.EntityComponentSystem.Events;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Networking.Events
{
    public class ClientDisconnectedEvent : IEvent
    {

        public IClient Client { get; set; }

        public ClientDisconnectedEvent(IClient client)
        {
            Client = client;
        }

    }
}

using CorgEng.GenericInterfaces.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Events.Events
{
    public class NetworkedEventRaisedEvent : IEvent
    {

        public INetworkedEvent RaisedEvent { get; set; }

        public NetworkedEventRaisedEvent(INetworkedEvent raisedEvent)
        {
            RaisedEvent = raisedEvent;
        }
    }
}

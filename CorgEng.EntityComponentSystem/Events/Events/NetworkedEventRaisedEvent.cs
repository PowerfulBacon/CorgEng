using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Events.Events
{
    public class NetworkedEventRaisedEvent : Event
    {

        public Event RaisedEvent { get; set; }

        //If this is made true, event will trigger an infinite loop and die
        public override bool NetworkedEvent => false;

        public NetworkedEventRaisedEvent(Event raisedEvent)
        {
            RaisedEvent = raisedEvent;
        }
    }
}

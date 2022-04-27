using CorgEng.EntityComponentSystem.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.InputHandling.Events
{
    public class MouseScrollEvent : Event
    {

        public double ScrollDelta { get; }

        public MouseScrollEvent(double scrollDelta)
        {
            ScrollDelta = scrollDelta;
        }
    }
}

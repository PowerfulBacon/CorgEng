using CorgEng.EntityComponentSystem.Events;
using CorgEng.UtilityTypes.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Implementations.Transform
{
    public sealed class SetPositionEvent : Event
    {

        public Vector<float> NewPosition { get; set; }

        public override bool NetworkedEvent => false;

        public SetPositionEvent(Vector<float> newPosition)
        {
            NewPosition = newPosition;
        }

    }
}

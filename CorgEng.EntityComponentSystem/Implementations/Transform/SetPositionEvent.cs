using CorgEng.EntityComponentSystem.Events;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.UtilityTypes.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Implementations.Transform
{
    public sealed class SetPositionEvent : IEvent
    {

        public Vector<float> NewPosition { get; set; }

        public SetPositionEvent(Vector<float> newPosition)
        {
            NewPosition = newPosition;
        }

        public SetPositionEvent()
        {
        }
    }
}

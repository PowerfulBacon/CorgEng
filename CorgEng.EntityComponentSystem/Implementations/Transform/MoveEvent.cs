using CorgEng.EntityComponentSystem.Events;
using CorgEng.UtilityTypes.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Implementations.Transform
{
    public class MoveEvent : Event
    {

        public Vector<float> OldPosition { get; set; }

        public Vector<float> NewPosition { get; set; }

        public MoveEvent(Vector<float> oldPosition, Vector<float> newPosition)
        {
            OldPosition = oldPosition;
            NewPosition = newPosition;
        }
    }
}

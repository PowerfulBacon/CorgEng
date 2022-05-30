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

        public double ScrollDelta { get; set; }

        public override bool IsSynced => true;

        public MouseScrollEvent(double scrollDelta)
        {
            ScrollDelta = scrollDelta;
        }

        public unsafe override byte[] Serialize()
        {
            double value = ScrollDelta;
            byte* bytePointer = (byte*)&value;
            return new byte[] {
                *bytePointer,
                *(bytePointer + 1),
                *(bytePointer + 2),
                *(bytePointer + 3),
                *(bytePointer + 4),
                *(bytePointer + 5),
                *(bytePointer + 6),
                *(bytePointer + 7)
            };
        }

        public unsafe override void Deserialize(byte[] data)
        {
            fixed (byte* dataPointer = data)
            {
                ScrollDelta = *(double*)dataPointer;
            }
        }
    }
}

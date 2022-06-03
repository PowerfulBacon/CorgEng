using CorgEng.EntityComponentSystem.Events;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.InputHandling.Events
{
    public class MouseScrollEvent : INetworkedEvent
    {

        public double ScrollDelta { get; set; }

        public MouseScrollEvent(double scrollDelta)
        {
            ScrollDelta = scrollDelta;
        }

        public unsafe byte[] Serialize()
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

        public unsafe void Deserialize(byte[] data)
        {
            fixed (byte* dataPointer = data)
            {
                ScrollDelta = *(double*)dataPointer;
            }
        }
    }
}

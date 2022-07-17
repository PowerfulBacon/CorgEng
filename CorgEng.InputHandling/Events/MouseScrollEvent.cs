using CorgEng.EntityComponentSystem.Events;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.InputHandling.Events
{
    public class MouseScrollEvent : INetworkedEvent
    {

        public bool CanBeRaisedByClient => true;

        public double ScrollDelta { get; set; }

        public MouseScrollEvent(double scrollDelta)
        {
            ScrollDelta = scrollDelta;
        }

        public void Deserialise(BinaryReader reader)
        {
            ScrollDelta = reader.ReadDouble();
        }

        public void Serialise(BinaryWriter writer)
        {
            writer.Write(ScrollDelta);
        }

        public int SerialisedLength()
        {
            return sizeof(double);
        }
    }
}

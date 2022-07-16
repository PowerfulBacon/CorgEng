using CorgEng.EntityComponentSystem.Events;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.UtilityTypes.Vectors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Implementations.Transform
{
    public class MoveEvent : INetworkedEvent
    {

        public bool CanBeRaisedByClient => false;

        public Vector<float> OldPosition { get; set; }

        public Vector<float> NewPosition { get; set; }

        public MoveEvent(Vector<float> oldPosition, Vector<float> newPosition)
        {
            OldPosition = oldPosition;
            NewPosition = newPosition;
        }

        public void Deserialise(BinaryReader reader)
        {
            OldPosition = new Vector<float>(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            NewPosition = new Vector<float>(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }

        public void Serialise(BinaryWriter writer)
        {
            writer.Write(OldPosition.X);
            writer.Write(OldPosition.Y);
            writer.Write(OldPosition.Dimensions > 2 ? OldPosition.Z : 0f);
            writer.Write(NewPosition.X);
            writer.Write(NewPosition.Y);
            writer.Write(NewPosition.Dimensions > 2 ? NewPosition.Z : 0f);
        }

        public int SerialisedLength()
        {
            return sizeof(float) * 6;
        }
    }
}

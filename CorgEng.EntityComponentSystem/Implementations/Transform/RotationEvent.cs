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
    internal class RotationEvent : INetworkedEvent
    {
        public bool CanBeRaisedByClient => false;

        public Vector<float> OldRotation { get; set; }

        public Vector<float> NewRotation { get; set; }

        public RotationEvent(Vector<float> oldRotation, Vector<float> newRotation)
        {
            OldRotation = oldRotation;
            NewRotation = newRotation;
        }

        public void Deserialise(BinaryReader reader)
        {
            OldRotation = new Vector<float>(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            NewRotation = new Vector<float>(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }

        public void Serialise(BinaryWriter writer)
        {
            writer.Write(OldRotation.X);
            writer.Write(OldRotation.Y);
            writer.Write(OldRotation.Dimensions > 2 ? OldRotation.Z : 0f);
            writer.Write(NewRotation.X);
            writer.Write(NewRotation.Y);
            writer.Write(NewRotation.Dimensions > 2 ? NewRotation.Z : 0f);
        }

        public int SerialisedLength()
        {
            return sizeof(float) * 6;
        }
    }
}

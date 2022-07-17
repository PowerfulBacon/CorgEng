using CorgEng.EntityComponentSystem.Events;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Implementations.Rendering.SpriteRendering
{
    public class SetSpriteEvent : INetworkedEvent
    {

        public bool CanBeRaisedByClient => false;

        public string TextureFile { get; set; }

        public SetSpriteEvent(string textureFile)
        {
            TextureFile = textureFile;
        }

        public void Deserialise(BinaryReader reader)
        {
            ushort length = reader.ReadUInt16();
            TextureFile = Encoding.ASCII.GetString(reader.ReadBytes(length));
        }

        public void Serialise(BinaryWriter writer)
        {
            writer.Write((ushort)TextureFile.Length);
            writer.Write(Encoding.ASCII.GetBytes(TextureFile));
        }

        public int SerialisedLength()
        {
            return Encoding.ASCII.GetByteCount(TextureFile) + sizeof(ushort);
        }
    }
}

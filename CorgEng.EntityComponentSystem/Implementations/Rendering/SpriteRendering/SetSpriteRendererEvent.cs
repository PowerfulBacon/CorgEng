using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Networking.Attributes;
using CorgEng.GenericInterfaces.Rendering.Renderers.SpriteRendering;
using CorgEng.GenericInterfaces.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Implementations.Rendering.SpriteRendering
{
    public class SetSpriteRendererEvent : INetworkedEvent
    {

        [UsingDependency]
        private static IAutoSerialiser AutoSerialiser;

        [NetworkSerialized]
        public uint Target { get; set; }

        public SetSpriteRendererEvent(uint target)
        {
            Target = target;
        }

        public SetSpriteRendererEvent(ISpriteRenderer target)
        {
            Target = target.NetworkIdentifier;
        }

        public void Deserialize(byte[] data)
        {
            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                using (BinaryReader binaryReader = new BinaryReader(memoryStream))
                {
                    Target = (uint)AutoSerialiser.Deserialize(typeof(uint), binaryReader);
                }
            }
        }

        public byte[] Serialize()
        {
            byte[] serializedRendererEvent = new byte[sizeof(uint)];
            using (MemoryStream memoryStream = new MemoryStream(serializedRendererEvent))
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
                {
                    AutoSerialiser.SerializeInto(typeof(uint), Target, binaryWriter);
                }
            }
            return serializedRendererEvent;
        }
    }
}

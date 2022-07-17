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

        public bool CanBeRaisedByClient => false;

        [UsingDependency]
        private static IAutoSerialiser AutoSerialiser;

        public uint Target { get; set; }

        public SetSpriteRendererEvent(uint target)
        {
            Target = target;
        }

        public SetSpriteRendererEvent(ISpriteRenderer target)
        {
            Target = target.NetworkIdentifier;
        }

        public void Deserialise(BinaryReader reader)
        {
            Target = reader.ReadUInt32();
        }

        public void Serialise(BinaryWriter writer)
        {
            writer.Write(Target);
        }

        public int SerialisedLength()
        {
            return sizeof(uint);
        }
    }
}

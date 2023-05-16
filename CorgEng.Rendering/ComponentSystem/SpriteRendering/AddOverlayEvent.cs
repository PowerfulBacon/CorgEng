using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Rendering.Icons;
using CorgEng.GenericInterfaces.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Implementations.Rendering.SpriteRendering
{
    public class AddOverlayEvent : INetworkedEvent
    {

        [UsingDependency]
        private static IAutoSerialiser AutoSerialiser;

        public bool CanBeRaisedByClient => false;

        public IIcon TextureFile { get; set; }

        public AddOverlayEvent(IIcon textureFile)
        {
            TextureFile = textureFile;
        }

        public void Deserialise(BinaryReader reader)
        {
            TextureFile = AutoSerialiser.Deserialize(typeof(IIcon), reader) as IIcon;
        }

        public void Serialise(BinaryWriter writer)
        {
            AutoSerialiser.SerializeInto(typeof(IIcon), TextureFile, writer);
        }

        public int SerialisedLength()
        {
            return AutoSerialiser.SerialisationLength(typeof(IIcon), TextureFile);
        }

    }
}

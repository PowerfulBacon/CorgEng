using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.Rendering.Icons;
using CorgEng.GenericInterfaces.Rendering.Textures;
using CorgEng.GenericInterfaces.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Rendering.Icons
{
    internal class Icon : IIcon
    {

        [UsingDependency]
        private static IAutoSerialiser AutoSerialiser;

        [UsingDependency]
        private static ITextureFactory TextureFactory;

        public string IconName { get; private set; }

        public float Layer { get; set; }

        /// <summary>
        /// Should this icon be rendered on the transparent rendering system?
        /// </summary>
        public bool HasTransparency => TextureFactory.GetIconStateTransparency(this);

        public Icon(string iconName, float layer)
        {
            IconName = iconName;
            Layer = layer;
        }

        public void DeserialiseFrom(BinaryReader binaryReader)
        {
            IconName = AutoSerialiser.Deserialize(typeof(string), binaryReader) as string;
            Layer = (float)AutoSerialiser.Deserialize(typeof(float), binaryReader);
        }

        public int GetSerialisationLength()
        {
            return AutoSerialiser.SerialisationLength(typeof(string), IconName) + AutoSerialiser.SerialisationLength(typeof(float), Layer);
        }

        public void SerialiseInto(BinaryWriter binaryWriter)
        {
            AutoSerialiser.SerializeInto(typeof(string), IconName, binaryWriter);
            AutoSerialiser.SerializeInto(typeof(float), Layer, binaryWriter);
        }

    }
}

using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.Rendering.Icons;
using CorgEng.GenericInterfaces.Rendering.Textures;
using CorgEng.GenericInterfaces.Serialization;
using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.UtilityTypes.Vectors;
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

        /// <summary>
        /// Triggered when a value is changed.
        /// </summary>
        public event Action ValueChanged;

        /// <summary>
        /// Name of the icon we are representing
        /// </summary>
        public string IconName { get; private set; }

        /// <summary>
        /// The layer to draw this icon on
        /// </summary>
        public float Layer { get; set; }

        /// <summary>
        /// Should this icon be rendered on the transparent rendering system?
        /// </summary>
        public bool HasTransparency => TextureFactory.GetIconStateTransparency(this);

        /// <summary>
        /// Directional state
        /// </summary>
        public DirectionalState DirectionalState { get; set; } = DirectionalState.NONE;

        /// <summary>
        /// Colour of the icon
        /// </summary>
        public IVector<float> Colour { get; set; } = new Vector<float>(1, 1, 1, 1);

        public Icon(string iconName, float layer)
        {
            IconName = iconName;
            Layer = layer;
            Colour.OnChange += (e, args) => {
                ValueChanged?.Invoke();
            };
        }

        public void DeserialiseFrom(BinaryReader binaryReader)
        {
            IconName = AutoSerialiser.Deserialize(typeof(string), binaryReader) as string;
            Layer = (float)AutoSerialiser.Deserialize(typeof(float), binaryReader);
            DirectionalState = (DirectionalState)AutoSerialiser.Deserialize(typeof(int), binaryReader);
        }

        public int GetSerialisationLength()
        {
            return AutoSerialiser.SerialisationLength(typeof(string), IconName)
                + AutoSerialiser.SerialisationLength(typeof(float), Layer)
                + AutoSerialiser.SerialisationLength(typeof(int), (int)DirectionalState);
        }

        public void SerialiseInto(BinaryWriter binaryWriter)
        {
            AutoSerialiser.SerializeInto(typeof(string), IconName, binaryWriter);
            AutoSerialiser.SerializeInto(typeof(float), Layer, binaryWriter);
            AutoSerialiser.SerializeInto(typeof(int), (int)DirectionalState, binaryWriter);
        }

    }
}

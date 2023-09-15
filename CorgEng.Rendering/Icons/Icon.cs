using CorgEng.Core;
using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.Rendering.Icons;
using CorgEng.GenericInterfaces.Rendering.Renderers.SpriteRendering;
using CorgEng.GenericInterfaces.Rendering.Textures;
using CorgEng.GenericInterfaces.Serialization;
using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.UtilityTypes.Matrices;
using CorgEng.UtilityTypes.Vectors;
using System;
using System.IO;

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
        /// The layer to draw this icon on. When updated, will trigger the ValueChanged
        /// callback.
        /// </summary>
        private float layer;

        public float Layer
        {
            get => layer;
            set
            {
                layer = value;
                ValueChanged?.Invoke();
            }
        }

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

        /// <summary>
        /// The renderer that should be used in order to render this icon.
        /// Will trigger ValueChanged when updated.
        /// </summary>
        private int plane;
        public int Plane
        {
            get => plane;
            set
            {
                plane = value;
                //Clear the cached renderer, it is no longer used.
                _cachedRenderer = null;
                ValueChanged?.Invoke();
            }
        }

        private ISpriteRenderer _cachedRenderer;

        public ISpriteRenderer Renderer
        {
            get
            {
                if (_cachedRenderer == null)
                    _cachedRenderer = (ISpriteRenderer)CorgEngMain.GetRendererForPlane(Plane);
                return _cachedRenderer;
            }
        }

        /// <summary>
        /// The transform of the icon
        /// </summary>
        private IMatrix _transform = Matrix.Identity[3];
        public IMatrix Transform
        {
            get => _transform;
            set
            {
                _transform = value;
                ValueChanged?.Invoke();
            }
        }

        public Icon(string iconName, float layer, int plane)
        {
            IconName = iconName;
            Layer = layer;
            Plane = plane;
            Colour.OnChange += (e, args) =>
            {
                ValueChanged?.Invoke();
            };
        }

        public void DeserialiseFrom(BinaryReader binaryReader)
        {
            IconName = AutoSerialiser.Deserialize(typeof(string), binaryReader) as string;
            Layer = (float)AutoSerialiser.Deserialize(typeof(float), binaryReader);
            Plane = (int)AutoSerialiser.Deserialize(typeof(int), binaryReader);
            DirectionalState = (DirectionalState)AutoSerialiser.Deserialize(typeof(int), binaryReader);
            Colour = (IVector<float>)AutoSerialiser.Deserialize(typeof(Vector<float>), binaryReader);
        }

        public int GetSerialisationLength()
        {
            return AutoSerialiser.SerialisationLength(typeof(string), IconName)
                + AutoSerialiser.SerialisationLength(typeof(float), Layer)
                + AutoSerialiser.SerialisationLength(typeof(int), Plane)
                + AutoSerialiser.SerialisationLength(typeof(int), (int)DirectionalState)
                + AutoSerialiser.SerialisationLength(typeof(Vector<float>), Colour);
        }

        public void SerialiseInto(BinaryWriter binaryWriter)
        {
            AutoSerialiser.SerializeInto(typeof(string), IconName, binaryWriter);
            AutoSerialiser.SerializeInto(typeof(float), Layer, binaryWriter);
            AutoSerialiser.SerializeInto(typeof(int), Plane, binaryWriter);
            AutoSerialiser.SerializeInto(typeof(int), (int)DirectionalState, binaryWriter);
            AutoSerialiser.SerializeInto(typeof(Vector<float>), Colour, binaryWriter);
        }

    }
}

using CorgEng.GenericInterfaces.Networking.Serialisation;
using CorgEng.GenericInterfaces.Rendering.Renderers.SpriteRendering;
using CorgEng.GenericInterfaces.Rendering.Textures;
using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Rendering.Icons
{
    public interface IIcon : ICustomSerialisationBehaviour
    {

        /// <summary>
        /// Called when the value of the icon changes
        /// </summary>
        event Action ValueChanged;

        /// <summary>
        /// The icon name to use. References an icon defined in a .texdef file.
        /// </summary>
        string IconName { get; }

        /// <summary>
        /// The layer that the icon should appear on
        /// </summary>
        float Layer { get; set; }

        /// <summary>
        /// Directional mode of the icon
        /// </summary>
        DirectionalState DirectionalState { get; set; }

        /// <summary>
        /// The colour of this icon
        /// </summary>
        IVector<float> Colour { get; set; }

        /// <summary>
        /// Does this texture have transparency?
        /// This only accounts for partial transparency (alpha values 1-254)
        /// </summary>
        bool HasTransparency { get; }

        /// <summary>
        /// The plane that this icon should be renderer on.
        /// </summary>
        uint Plane { get; set; }

        /// <summary>
        /// The renderer being used to render this icon.
        /// Will be whichever one is attached to the icon's plane.
        /// </summary>
        ISpriteRenderer Renderer { get; }

        /// <summary>
        /// The transform that will be applied to this icon.
        /// </summary>
        IMatrix Transform { get; set; }

    }
}

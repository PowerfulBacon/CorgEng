using CorgEng.GenericInterfaces.Networking.Serialisation;
using CorgEng.GenericInterfaces.Rendering.Textures;
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
        /// Does this texture have transparency?
        /// This only accounts for partial transparency (alpha values 1-254)
        /// </summary>
        bool HasTransparency { get; }

    }
}

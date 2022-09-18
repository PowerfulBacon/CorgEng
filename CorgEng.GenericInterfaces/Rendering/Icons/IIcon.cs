using CorgEng.GenericInterfaces.Networking.Serialisation;
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

    }
}

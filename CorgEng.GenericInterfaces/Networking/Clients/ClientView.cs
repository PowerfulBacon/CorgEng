using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Networking.Clients
{
    /// <summary>
    /// Holder for information about the client view.
    /// </summary>
    public class ClientView
    {

        /// <summary>
        /// The X offset of the client view from the client entity.
        /// </summary>
        public double ViewOffsetX { get; set; }

        /// <summary>
        /// The Y offset of the client view from the client entity
        /// </summary>
        public double ViewOffsetY { get; set; }

        /// <summary>
        /// The width of the client view
        /// </summary>
        public double ViewWidth { get; set; }

        /// <summary>
        /// The height of this client view
        /// </summary>
        public double ViewHeight { get; set; }

    }
}

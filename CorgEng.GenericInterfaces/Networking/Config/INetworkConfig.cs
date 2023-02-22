using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Networking.Config
{
    public interface INetworkConfig
    {

        bool NetworkingActive { get; set; }

        bool ProcessServerSystems { get; set; }

        bool ProcessClientSystems { get; set; }

        int TickRate { set; get; }

        int PacketMaxSizeBytes { set; get; }

#if DEBUG
        /// <summary>
        /// Probablility that packets will be dropped for no reason.
        /// Please don't use this in production!!
        /// </summary>
        public double PacketDropProbability { get; }
#endif

    }
}

using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.Networking.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Networking.Config
{
    /// <summary>
    /// Increased priority to replace default config
    /// </summary>
    [Dependency(priority = 2)]
    internal class NetworkConfig : INetworkConfig
    {

        private static int _tickrate = 32;

        public int TickRate
        {
            set => _tickrate = value;
            get => _tickrate;
        }

        private static int _packetMaxSizeBytes = 65507;

        public int PacketMaxSizeBytes
        {
            set => _packetMaxSizeBytes = value;
            get => _packetMaxSizeBytes;
        }

        private static bool _processServerSystems = false;

        public bool ProcessServerSystems { get => _processServerSystems; set => _processServerSystems = value; }

        private static bool _processClientSystems = false;

        public bool ProcessClientSystems { get => _processClientSystems; set => _processClientSystems = value; }

        private static bool _networkingActive = false;

        public bool NetworkingActive { get => _networkingActive; set => _networkingActive = value; }

#if DEBUG
        /// <summary>
        /// Probablility that packets will be dropped for no reason.
        /// Please don't use this in production!!
        /// </summary>
        public double PacketDropProbability => 0;
#endif

    }
}

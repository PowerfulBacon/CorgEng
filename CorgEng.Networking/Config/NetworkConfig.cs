using CorgEng.GenericInterfaces.Networking.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Networking.Config
{
    public class NetworkConfig : INetworkConfig
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
    }
}

using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.Networking.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Networking.Packets
{
    [Dependency]
    internal class NetworkMessageFactory : INetworkMessageFactory
    {

        public INetworkMessage CreateMessage(PacketHeaders packetHeader, byte[] content)
        {
            return new NetworkMessage(packetHeader, content);
        }

        public INetworkMessage CreateMessage(PacketHeaders packetHeader, string content)
        {
            return CreateMessage(packetHeader, Encoding.ASCII.GetBytes(content));
        }

    }
}

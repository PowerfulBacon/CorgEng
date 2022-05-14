using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Networking.Packets
{
    public interface INetworkMessageFactory
    {

        INetworkMessage CreateMessage(PacketHeaders packetHeader, byte[] content);

        INetworkMessage CreateMessage(PacketHeaders packetHeader, string content);

    }
}

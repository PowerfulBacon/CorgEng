using CorgEng.GenericInterfaces.Networking.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Networking.Packets.PacketQueues
{
    public interface IQueuedPacketFactory
    {

        IQueuedPacket CreatePacket(IClientAddress targets, byte[] data);

    }
}

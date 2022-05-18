using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.Networking.Clients;
using CorgEng.GenericInterfaces.Networking.Packets.PacketQueues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Networking.Packets.PacketQueues
{
    [Dependency]
    internal class QueuedPacketFactory : IQueuedPacketFactory
    {

        public IQueuedPacket CreatePacket(IClientAddress targets, byte[] data)
        {
            return new QueuedPacket(targets, data);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Networking.Packets.PacketQueues
{
    public interface IPacketQueueFactory
    {

        IPacketQueue CreatePacketQueue();

    }
}

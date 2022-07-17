using CorgEng.GenericInterfaces.Networking.Packets.PacketQueues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Networking.Packets
{
    public interface INetworkMessage
    {

        /// <summary>
        /// The length of the network message in bytes
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Get the bytes of this message
        /// </summary>
        byte[] GetBytes();

        /// <summary>
        /// Insert the bytes of our message into another array
        /// at the specified position.
        /// More ideal than getting the bytes and copying the array.
        /// </summary>
        void InsertBytes(IQueuedPacket target);

    }
}

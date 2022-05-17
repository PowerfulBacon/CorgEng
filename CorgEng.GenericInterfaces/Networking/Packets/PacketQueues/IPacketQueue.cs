using CorgEng.GenericInterfaces.Networking.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Networking.Packets.PacketQueues
{
    /// <summary>
    /// Needs to be thread safe
    /// </summary>
    public interface IPacketQueue
    {

        /// <summary>
        /// Queue a message to be sent to the server.
        /// Contains the IPAddress of the target so messages can be bundled together
        /// </summary>
        void QueueMessage(IClientAddress targets, INetworkMessage message);

        /// <summary>
        /// Do we have any more messages to send?
        /// </summary>
        bool HasMessages();

        /// <summary>
        /// Get the next packet that needs to be sent
        /// </summary>
        IQueuedPacket DequeuePacket();

    }
}

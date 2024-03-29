﻿using CorgEng.GenericInterfaces.Networking.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Networking.Packets.PacketQueues
{
    public interface IQueuedPacket
    {

        /// <summary>
        /// The top of the data pointer
        /// </summary>
        int TopPointer { get; set; }

        /// <summary>
        /// The data inside the queued packet
        /// </summary>
        byte[] Data { get; }

        /// <summary>
        /// The targets of this queued packet
        /// </summary>
        IClientAddress Targets { get; }

        /// <summary>
        /// Defines when this packet was transmitted at, used for verification purposes
        /// </summary>
        double SentAt { get; set; }

        /// <summary>
        /// The identifier of this packet, used to confirm its arrival
        /// </summary>
        int PacketIdentifier { get; }

        /// <summary>
        /// Can we insert a message of some specified length
        /// into this message?
        /// </summary>
        bool CanInsert(int length);

    }
}

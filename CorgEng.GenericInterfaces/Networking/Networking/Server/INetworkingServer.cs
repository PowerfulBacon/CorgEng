﻿using CorgEng.GenericInterfaces.Networking.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Networking.Networking.Server
{
    public interface INetworkingServer
    {

        /// <summary>
        /// The tick rate of the server
        /// How many times per second will packets be sent off.
        /// </summary>
        int TickRate { get; }

        /// <summary>
        /// Start hosting the server on the specified port.
        /// </summary>
        void StartHosting(int port);

        /// <summary>
        /// Queue a message to send
        /// </summary>
        void QueueMessage(INetworkMessage message);

    }
}
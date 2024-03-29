﻿using CorgEng.GenericInterfaces.Networking.Clients;
using CorgEng.GenericInterfaces.Networking.Packets;
using CorgEng.GenericInterfaces.Networking.PrototypeManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Networking.Networking.Server
{
    public interface INetworkServer
    {

        /// <summary>
        /// Called every time a network message is received
        /// </summary>
        event NetworkMessageRecieved NetworkMessageReceived;

        /// <summary>
        /// The addressing table of the server
        /// </summary>
        IClientAddressingTable ClientAddressingTable { get; }

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
        void QueueMessage(IClientAddress targets, INetworkMessage message);

        /// <summary>
        /// Terminate the networking and cleanup the server
        /// </summary>
        void Cleanup();

        /// <summary>
        /// Set the prototype that gets created when somebody joins
        /// </summary>
        /// <param name="prototype"></param>
        void SetClientPrototype(IPrototype prototype);

    }
}

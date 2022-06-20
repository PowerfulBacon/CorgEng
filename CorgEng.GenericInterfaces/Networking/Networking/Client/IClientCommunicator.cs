using CorgEng.GenericInterfaces.Networking.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Networking.Networking.Client
{
    public interface IClientCommunicator
    {

        /// <summary>
        /// Send a message to the server.
        /// </summary>
        /// <param name="networkMessage"></param>
        void SendToServer(INetworkMessage networkMessage);

    }
}

using CorgEng.GenericInterfaces.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Networking.Clients
{
    internal class Client : IClient
    {

        public IPEndPoint ClientEndPoint { get; }

        public string Username { get; }

        public Client(string username, IPEndPoint clientEndPoint)
        {
            Username = username;
            ClientEndPoint = clientEndPoint;
        }

        public void Ban(string banReason)
        {
            throw new NotImplementedException();
        }

        public void Disconnect(string disconnectReason)
        {
            throw new NotImplementedException();
        }

        public void SendMessage(UdpClient udpClient, byte[] message, int amount)
        {
            udpClient.SendAsync(message, amount, ClientEndPoint);
        }

    }
}

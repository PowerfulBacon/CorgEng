using CorgEng.GenericInterfaces.Networking.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Networking.Packets
{
    public class NetworkMessage : INetworkMessage
    {

        byte[] content;

        public NetworkMessage(PacketHeaders packetHeader, byte[] packetContent)
        {
            content = new byte[6 + packetContent.Length];
            BitConverter.GetBytes((int)packetHeader).CopyTo(content, 0);
            BitConverter.GetBytes((short)packetContent.Length).CopyTo(content, 4);
            packetContent.CopyTo(content, 6);
        }

        public byte[] GetBytes()
        {
            //First 4 bytes: Packet header
            //Next 2 bytes: Packet Length
            //Rest: packet contents
            return content;
        }

    }
}

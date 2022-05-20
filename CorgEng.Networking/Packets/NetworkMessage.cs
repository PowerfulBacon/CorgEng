using CorgEng.GenericInterfaces.Networking.Packets;
using CorgEng.GenericInterfaces.Networking.Packets.PacketQueues;
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
            BitConverter.GetBytes((short)packetContent.Length).CopyTo(content, 0x00);
            BitConverter.GetBytes((int)packetHeader).CopyTo(content, 0x02);
            packetContent.CopyTo(content, 6);
        }

        public int Length => content.Length;

        public byte[] GetBytes()
        {
            return content;
        }

        public void InsertBytes(IQueuedPacket target)
        {
            content.CopyTo(target.Data, target.TopPointer);
            target.TopPointer += Length;
        }
    }
}

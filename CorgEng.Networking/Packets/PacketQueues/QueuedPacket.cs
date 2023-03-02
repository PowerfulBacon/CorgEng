using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.Networking.Clients;
using CorgEng.GenericInterfaces.Networking.Config;
using CorgEng.GenericInterfaces.Networking.Packets.PacketQueues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Networking.Packets.PacketQueues
{
    public class QueuedPacket : IQueuedPacket
    {

        [UsingDependency]
        private static INetworkConfig NetworkConfig;

        public IClientAddress Targets { get; }

        public byte[] Data { get; }

        public int TopPointer { get; set; }

        private static int PacketCount = 0;

        public int PacketIdentifier { get; private set; }

        public double SentAt { get; set; }

        public QueuedPacket(IClientAddress targets)
        {
            PacketIdentifier = PacketCount++;
            Targets = targets;
            Data = new byte[NetworkConfig.PacketMaxSizeBytes];
            BitConverter.GetBytes(PacketIdentifier).CopyTo(Data, 0);
            TopPointer = 8;
        }

        public QueuedPacket(IClientAddress targets, byte[] data)
        {
            PacketIdentifier = PacketCount++;
            Targets = targets;
            Data = new byte[NetworkConfig.PacketMaxSizeBytes];
            BitConverter.GetBytes(PacketIdentifier).CopyTo(Data, 0);
            data.CopyTo(Data, 8);
            TopPointer = data.Length + 8;
        }

        public bool CanInsert(int length)
        {
            return Data.Length - TopPointer >= length;
        }

    }
}

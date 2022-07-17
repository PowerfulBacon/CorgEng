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

        public QueuedPacket(IClientAddress targets)
        {
            Targets = targets;
            Data = new byte[NetworkConfig.PacketMaxSizeBytes];
            TopPointer = 0;
        }

        public QueuedPacket(IClientAddress targets, byte[] data)
        {
            Targets = targets;
            Data = new byte[NetworkConfig.PacketMaxSizeBytes];
            data.CopyTo(Data, 0);
            TopPointer = data.Length;
        }

        public bool CanInsert(int length)
        {
            return Data.Length - TopPointer >= length;
        }

    }
}

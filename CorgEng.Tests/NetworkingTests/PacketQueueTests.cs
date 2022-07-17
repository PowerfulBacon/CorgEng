using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.Networking.Clients;
using CorgEng.GenericInterfaces.Networking.Packets;
using CorgEng.GenericInterfaces.Networking.Packets.PacketQueues;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Tests.NetworkingTests
{
    [TestClass]
    public class PacketQueueTests
    {

        [UsingDependency]
        private static IPacketQueueFactory PacketQueueFactory;

        [UsingDependency]
        private static IQueuedPacketFactory QueuedPacketFactory;

        [UsingDependency]
        private static IClientAddressFactory ClientAddressFactory;

        [UsingDependency]
        private static INetworkMessageFactory NetworkMessageFactory;

        [TestMethod]
        [Timeout(500)]
        public void TestPacketQueue()
        {
            IClientAddress clientAddress = ClientAddressFactory.CreateEmptyAddress();
            IPacketQueue packetQueue = PacketQueueFactory.CreatePacketQueue();
            //Should be empty
            Assert.IsFalse(packetQueue.AcquireLockIfHasMessages());
            //Add a packet
            INetworkMessage networkMessage = NetworkMessageFactory.CreateMessage(PacketHeaders.NETWORKING_TEST, new byte[] { });
            packetQueue.QueueMessage(clientAddress, networkMessage);
            Assert.IsTrue(packetQueue.AcquireLockIfHasMessages());
            packetQueue.ReleaseLock();
            packetQueue.AcquireLockIfHasMessages();
            IQueuedPacket queuedPacket = packetQueue.DequeuePacket();
            packetQueue.ReleaseLock();
            Assert.IsNotNull(queuedPacket);
            Assert.IsFalse(packetQueue.AcquireLockIfHasMessages());
            INetworkMessage networkMessageA = NetworkMessageFactory.CreateMessage(PacketHeaders.NETWORKING_TEST, new byte[] { 4, 18, 80 });
            INetworkMessage networkMessageB = NetworkMessageFactory.CreateMessage(PacketHeaders.NETWORKING_TEST, new byte[] { 1, 2, 3, 4, 5, 6 });
            packetQueue.QueueMessage(clientAddress, networkMessageA);
            packetQueue.QueueMessage(clientAddress, networkMessageB);
            Assert.IsTrue(packetQueue.AcquireLockIfHasMessages());
            packetQueue.ReleaseLock();
            while (packetQueue.AcquireLockIfHasMessages())
            {
                packetQueue.DequeuePacket();
                packetQueue.ReleaseLock();
            }
            Assert.IsFalse(packetQueue.AcquireLockIfHasMessages());
            packetQueue.QueueMessage(null, networkMessageA);
            packetQueue.QueueMessage(null, networkMessageB);
            Assert.IsTrue(packetQueue.AcquireLockIfHasMessages());
            packetQueue.ReleaseLock();
            while (packetQueue.AcquireLockIfHasMessages())
            {
                packetQueue.DequeuePacket();
                packetQueue.ReleaseLock();
            }
            Assert.IsFalse(packetQueue.AcquireLockIfHasMessages());
            for (int i = 0; i < 70000; i++)
            {
                packetQueue.QueueMessage(null, networkMessageA);
                packetQueue.QueueMessage(null, networkMessageB);
            }
            Assert.IsTrue(packetQueue.AcquireLockIfHasMessages()); ;
            packetQueue.ReleaseLock();
            while (packetQueue.AcquireLockIfHasMessages())
            {
                packetQueue.DequeuePacket(); ;
                packetQueue.ReleaseLock();
            }
            Assert.IsFalse(packetQueue.AcquireLockIfHasMessages());
        }

    }
}

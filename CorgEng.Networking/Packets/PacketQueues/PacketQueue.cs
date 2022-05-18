using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Networking.Clients;
using CorgEng.GenericInterfaces.Networking.Packets;
using CorgEng.GenericInterfaces.Networking.Packets.PacketQueues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Networking.Packets.PacketQueues
{
    public class PacketQueue : IPacketQueue
    {

        [UsingDependency]
        private static IQueuedPacketFactory QueuedPacketFactory;

        [UsingDependency]
        private static ILogger Logger;

        private Dictionary<IClientAddress, Stack<IQueuedPacket>> queuedPackets = new Dictionary<IClientAddress, Stack<IQueuedPacket>>();

        public IQueuedPacket DequeuePacket()
        {
            lock (queuedPackets)
            {
                //Dequeue from top stack
                KeyValuePair<IClientAddress, Stack<IQueuedPacket>> topEntry = queuedPackets.First();
                Stack<IQueuedPacket> topStack = topEntry.Value;
                //Error case
                if (topStack.Count == 0)
                {
                    queuedPackets.Remove(topEntry.Key);
                    throw new Exception("Packet queue contained an empty stack.");
                }
                IQueuedPacket firstPacket = topStack.Pop();
                if (topStack.Count == 0)
                    queuedPackets.Remove(topEntry.Key);
                return firstPacket;
            }
        }

        public bool HasMessages()
        {
            lock (queuedPackets)
            {
                return queuedPackets.Count > 0;
            }
        }

        public void QueueMessage(IClientAddress targets, INetworkMessage message)
        {
            lock (queuedPackets)
            {
                //Create the new queue
                if (!queuedPackets.ContainsKey(targets))
                {
                    queuedPackets.Add(targets, new Stack<IQueuedPacket>());
                }
                //If there are no elements, queue directly
                Stack<IQueuedPacket> packetStack = queuedPackets[targets];
                if (packetStack.Count == 0)
                {
                    packetStack.Push(QueuedPacketFactory.CreatePacket(targets, message.GetBytes()));
                    return;
                }
                //Pull the top element of the queue
                IQueuedPacket topPacket = packetStack.Peek();
                //Can we add our data into this packet?
                if (topPacket.CanInsert(message.Length))
                {
                    message.InsertBytes(topPacket);
                }
                else
                {
                    //Queue a new message
                    packetStack.Push(QueuedPacketFactory.CreatePacket(targets, message.GetBytes()));
                }
            }
        }

    }
}

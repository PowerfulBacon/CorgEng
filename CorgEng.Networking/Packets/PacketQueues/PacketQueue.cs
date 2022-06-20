using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Networking.Clients;
using CorgEng.GenericInterfaces.Networking.Packets;
using CorgEng.GenericInterfaces.Networking.Packets.PacketQueues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        private Stack<IQueuedPacket> untargettedPackets = new Stack<IQueuedPacket>();

        private object lockObject = new object();

        public IQueuedPacket DequeuePacket()
        {
            if (!Monitor.IsEntered(lockObject))
            {
                throw new Exception("Attempting to dequeue a packet without an acquired lock!");
            }
            if (untargettedPackets.Count > 0)
            {
                return untargettedPackets.Pop();
            }
            //Dequeue from top stack
            IClientAddress firstKey = queuedPackets.Keys.First();
            Stack<IQueuedPacket> topStack = queuedPackets[firstKey];
            //Error case
            if (topStack.Count == 0)
            {
                queuedPackets.Remove(firstKey);
                throw new Exception("Packet queue contained an empty stack.");
            }
            IQueuedPacket firstPacket = topStack.Pop();
            if (topStack.Count == 0)
                queuedPackets.Remove(firstKey);
            return firstPacket;
        }

        public bool AcquireLockIfHasMessages()
        {
            Monitor.Enter(lockObject);
            if (queuedPackets.Count > 0 || untargettedPackets.Count > 0)
            {
                return true;
            }
            Monitor.Exit(lockObject);
            return false;
        }

        public void ReleaseLock()
        {
            Monitor.Exit(lockObject);
        }

        public void QueueMessage(IClientAddress targets, INetworkMessage message)
        {
            bool hasLock = false;
            Stack<IQueuedPacket> packetStack;
            IQueuedPacket topPacket;
            if (Monitor.IsEntered(lockObject))
            {
                hasLock = true;
            }
            else
            {
                Monitor.Enter(lockObject);
            }
            try
            {
                if (targets == null)
                {
                    //If there are no elements, queue directly
                    packetStack = untargettedPackets;
                    if (packetStack.Count == 0)
                    {
                        packetStack.Push(QueuedPacketFactory.CreatePacket(null, message.GetBytes()));
                        return;
                    }
                    //Pull the top element of the queue
                    topPacket = packetStack.Peek();
                    //Can we add our data into this packet?
                    if (topPacket.CanInsert(message.Length))
                    {
                        message.InsertBytes(topPacket);
                    }
                    else
                    {
                        //Queue a new message
                        packetStack.Push(QueuedPacketFactory.CreatePacket(null, message.GetBytes()));
                    }
                    return;
                }
                //Create the new queue
                if (!queuedPackets.ContainsKey(targets))
                {
                    queuedPackets.Add(targets, new Stack<IQueuedPacket>());
                }
                //If there are no elements, queue directly
                packetStack = queuedPackets[targets];
                if (packetStack.Count == 0)
                {
                    packetStack.Push(QueuedPacketFactory.CreatePacket(targets, message.GetBytes()));
                    return;
                }
                //Pull the top element of the queue
                topPacket = packetStack.Peek();
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
            finally
            {
                if (!hasLock)
                {
                    Monitor.Exit(lockObject);
                }
            }
        }

    }
}

using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.Networking.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Networking.Clients
{
    [Dependency]
    internal class ClientAddressingTable : IClientAddressingTable
    {

        private int maxValue = 0;

        IClientAddress everyone = new ClientAddress(0);

        private Dictionary<IPAddress, IClientAddress> clientAddresses = new Dictionary<IPAddress, IClientAddress>();

        private List<IClientAddress> unusedAddresses = new List<IClientAddress>();

        public IClientAddress AddAddress(IPAddress address)
        {
            //Locate the first free value
            if (unusedAddresses.Count > 0)
            {
                //Allocate the first address
                IClientAddress firstAddress = unusedAddresses[0];
                unusedAddresses.RemoveAt(0);
                clientAddresses.Add(address, firstAddress);
                return firstAddress;
            }
            //Allocate a new address
            ClientAddress allocated = new ClientAddress(++maxValue);
            clientAddresses.Add(address, allocated);
            everyone.EnableFlag(allocated);
            return allocated;
        }

        /// <summary>
        /// Remove all registered client addresses
        /// </summary>
        public void Clear()
        {
            for (int i = clientAddresses.Count - 1; i >= 0; i--)
            {
                RemoveAddress(clientAddresses.Keys.ElementAt(i));
            }
        }

        public IClientAddress GetEveryone()
        {
            return everyone;
        }

        public IClientAddress GetFlagRepresentation(IPAddress address)
        {
            return clientAddresses[address];
        }

        public void RemoveAddress(IPAddress address)
        {
            IClientAddress clientAddress = clientAddresses[address];
            unusedAddresses.Add(clientAddress);
            clientAddresses.Remove(address);
        }

        public void Reset()
        {
            Clear();
            maxValue = 0;
            everyone = new ClientAddress(0);
            clientAddresses = new Dictionary<IPAddress, IClientAddress>();
            unusedAddresses = new List<IClientAddress>();
        }
    }
}

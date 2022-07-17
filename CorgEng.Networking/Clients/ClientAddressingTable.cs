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
    internal class ClientAddressingTable : IClientAddressingTable
    {

        private int maxValue = 0;

        IClientAddress everyone = new ClientAddress(0, null);

        private Dictionary<IClient, IClientAddress> clientAddresses = new Dictionary<IClient, IClientAddress>();

        private List<IClientAddress> unusedAddresses = new List<IClientAddress>();

        public IClientAddress AddClient(IClient client)
        {
            //Locate the first free value
            if (unusedAddresses.Count > 0)
            {
                //Allocate the first address
                IClientAddress firstAddress = unusedAddresses[0];
                unusedAddresses.RemoveAt(0);
                clientAddresses.Add(client, firstAddress);
                return firstAddress;
            }
            //Allocate a new address
            ClientAddress allocated = new ClientAddress(++maxValue, client);
            clientAddresses.Add(client, allocated);
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
                RemoveClient(clientAddresses.Keys.ElementAt(i));
            }
        }

        public IClientAddress GetEveryone()
        {
            return everyone;
        }

        public IClientAddress GetFlagRepresentation(IClient client)
        {
            return clientAddresses[client];
        }

        public void RemoveClient(IClient client)
        {
            IClientAddress clientAddress = clientAddresses[client];
            unusedAddresses.Add(clientAddress);
            clientAddresses.Remove(client);
        }

        public void Reset()
        {
            Clear();
            maxValue = 0;
            everyone = new ClientAddress(0, null);
            clientAddresses = new Dictionary<IClient, IClientAddress>();
            unusedAddresses = new List<IClientAddress>();
        }
    }
}

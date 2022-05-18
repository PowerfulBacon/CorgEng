using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Networking.Clients
{
    /// <summary>
    /// Each client gets a unique ID upon joining
    /// </summary>
    public interface IClientAddressingTable
    {

        /// <summary>
        /// Add an address to the addressing table
        /// </summary>
        IClientAddress AddAddress(IPAddress address);

        /// <summary>
        /// Remove an IP address from 
        /// </summary>
        void RemoveAddress(IPAddress address);

        /// <summary>
        /// Get a pointer to the first byte and the length in bytes of
        /// the flag representation value of an address.
        /// </summary>
        IClientAddress GetFlagRepresentation(IPAddress address);

        /// <summary>
        /// Get the client address that represents everyone.
        /// </summary>
        IClientAddress GetEveryone();

        /// <summary>
        /// Clear the addressing table (For disconnect scenarios)
        /// </summary>
        void Clear();

        /// <summary>
        /// Completely reset the addressing table
        /// </summary>
        void Reset();

    }
}

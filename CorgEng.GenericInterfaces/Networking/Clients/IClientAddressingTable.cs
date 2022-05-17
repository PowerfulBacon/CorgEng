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
    public unsafe interface IClientAddressingTable
    {

        /// <summary>
        /// Add an address to the addressing table
        /// </summary>
        void AddAddress(IPAddress address);

        /// <summary>
        /// Get a pointer to the first byte and the length in bytes of
        /// the flag representation value of an address.
        /// </summary>
        byte* GetFlagRepresentation(IPAddress address, out int length);

    }
}

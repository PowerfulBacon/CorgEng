using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Networking.Clients
{
    /// <summary>
    /// MUST OVERRIDE EQUALS AND GENERATE HASH
    /// </summary>
    public unsafe interface IClientAddress
    {

        byte* AddressPointer { get; }

        int AddressBytes { get; }

    }
}

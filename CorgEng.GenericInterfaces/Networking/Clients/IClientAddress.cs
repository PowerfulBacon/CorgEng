﻿using System;
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

        bool HasTargets { get; }

        byte[] ByteArray { get; }

        int AddressBytes { get; }

        bool HasFlag(IClientAddress searchingFor);

        void EnableFlag(IClientAddress enablingFlag);

        void DisableFlag(IClientAddress disablingFlag);

        IEnumerable<IClient> GetClients();

    }
}

using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.Networking.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Networking.Clients
{
    [Dependency]
    internal class ClientAddressingTableFactory : IClientAddressingTableFactory
    {
        public IClientAddressingTable CreateAddressingTable()
        {
            return new ClientAddressingTable();
        }
    }
}

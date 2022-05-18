using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Networking.Clients
{
    public interface IClientAddressFactory
    {

        IClientAddress CreateEmptyAddress();

        IClientAddress CreateAddress(int clientId);

    }
}

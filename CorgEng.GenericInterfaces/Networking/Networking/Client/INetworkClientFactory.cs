using CorgEng.GenericInterfaces.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Networking.Networking.Client
{
    public interface INetworkClientFactory
    {

        INetworkClient CreateNetworkClient(IWorld world);

    }
}

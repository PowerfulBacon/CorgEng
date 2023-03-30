using CorgEng.GenericInterfaces.Networking.Networking.Client;
using CorgEng.GenericInterfaces.Networking.Networking.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.EntityComponentSystem
{
    public interface IWorld
    {

        IEntityManager EntityManager { get; }

        IEntitySystemManager EntitySystemManager { get; }

        INetworkServer ServerInstance { get; }

        INetworkClient ClientInstance { get; }

    }
}

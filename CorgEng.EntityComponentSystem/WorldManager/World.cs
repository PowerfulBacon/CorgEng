using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Systems;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Networking.Networking.Client;
using CorgEng.GenericInterfaces.Networking.Networking.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.WorldManager
{
    /// <summary>
    /// Represents an entity group container.
    /// Different worlds represent different processes
    /// that do not directly interact with each other.
    /// Primarilly used for unit testing.
    /// </summary>
    internal class World : IWorld
    {

        [UsingDependency]
        private static INetworkServerFactory NetworkServerFactory = null!;

        [UsingDependency]
        private static INetworkClientFactory NetworkClientFactory = null!;

        /// <summary>
        /// The entity manager associated with this world process
        /// </summary>
        public IEntityManager EntityManager { get; }

        /// <summary>
        /// The entity system manager associated with this world process
        /// </summary>
        public IEntitySystemManager EntitySystemManager { get; }

        /// <summary>
        /// The server instance, if it doesn't exist for this world, one will be
        /// created.
        /// </summary>
        private INetworkServer _serverInstance;
        public INetworkServer ServerInstance
        {
            get
            {
                if (_serverInstance == null)
                    _serverInstance = NetworkServerFactory.CreateNetworkServer(this);
                return _serverInstance;
            }
        }
        
        /// <summary>
        /// The client instance, if one doesn't exist for this world, one will be
        /// created.
        /// </summary>
        private INetworkClient _clientInstance;
        public INetworkClient ClientInstance
        {
            get
            {
                if (_clientInstance == null)
                    _clientInstance = NetworkClientFactory.CreateNetworkClient(this);
                return _clientInstance;
            }
        }

        public World()
        {
            EntityManager = new EntityManager(this);
            EntitySystemManager systemManager = new EntitySystemManager(this);
            EntitySystemManager = systemManager;
            systemManager.CreateAllSystems();
        }

        /// <summary>
        /// Cleanup the server and client instance when the world goes out of testing scope.
        /// </summary>
        ~World()
        {
            _serverInstance?.Cleanup();
            _clientInstance?.Cleanup();
        }

    }
}

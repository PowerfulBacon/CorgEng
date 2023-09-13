using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Implementations.Transform;
using CorgEng.GenericInterfaces.Networking.Clients;
using CorgEng.GenericInterfaces.Networking.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Networking.Components
{
    /// <summary>
    /// A derivative of transform component that will allow for the transform
    /// to be synced to clients.
    /// </summary>
    public sealed class NetworkTransformComponent : TransformComponent
    {

        [UsingDependency]
        private static INetworkConfig NetworkConfig;

        /// <summary>
        /// Automatically assign ownership on creation if we are the server.
        /// </summary>
        private bool _hasLocalOwnership = !NetworkConfig.NetworkingActive || NetworkConfig.ProcessServerSystems;

        /// <summary>
        /// Are we the management of ownership?
        /// </summary>
        private bool _isOwnershipManager = !NetworkConfig.NetworkingActive || NetworkConfig.ProcessServerSystems;

        /// <summary>
        /// The current owner of this transform component as determined by the ownership manager.
        /// </summary>
        internal IClient currentOwner;

        /// <summary>
        /// Returns true if the local application has ownership of this network transform.
        /// </summary>
        public bool HasOwnership
        {
            get
            {
                return _hasLocalOwnership;
            }
        }

        /// <summary>
        /// Are we the set ownership manager?
        /// The ownership manager will be the server, they are the one that
        /// verifies any attempted modifications of the entity.
        /// </summary>
        public bool IsOwnershipManager
        {
            get { return _isOwnershipManager; }
        }

    }
}

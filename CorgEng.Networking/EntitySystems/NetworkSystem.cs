using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.EntityComponentSystem.Implementations.Transform;
using CorgEng.EntityComponentSystem.Systems;
using CorgEng.GenericInterfaces.Networking.Networking;
using CorgEng.GenericInterfaces.Networking.Packets;
using CorgEng.UtilityTypes.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Networking.EntitySystems
{
    internal class NetworkSystem : EntitySystem
    {

        [UsingDependency]
        private static IServerCommunicator ServerCommunicator;

        [UsingDependency]
        private INetworkMessageFactory NetworkMessageFactory;

        public override void SystemSetup()
        {
            RegisterLocalEvent<TransformComponent, NetworkedEventRaisedEvent>(OnNetworkedEventRaised);
        }

        /// <summary>
        /// Triggered when a networked event was raised.
        /// Uses the transform component to determine if the entity is near a camera,
        /// so any networked events need to has a transform component on the entity.
        /// </summary>
        private void OnNetworkedEventRaised(Entity entity, TransformComponent transformComponent, NetworkedEventRaisedEvent networkedEventRaisedEvent)
        {
            if (ServerCommunicator?.IsServer ?? false)
            {
                ServerCommunicator.SendToReleventClients(
                    NetworkMessageFactory.CreateMessage(PacketHeaders.EVENT_RAISED, networkedEventRaisedEvent.Serialize()),
                    transformComponent.Position,
                    new Vector<float>(1, 1, 1)
                    );
            }
        }

    }
}

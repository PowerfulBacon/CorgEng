using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.EntityComponentSystem.Implementations.Transform;
using CorgEng.EntityComponentSystem.Systems;
using CorgEng.GenericInterfaces.Networking.Networking;
using CorgEng.GenericInterfaces.Networking.Packets;
using CorgEng.Networking.Components;
using CorgEng.Networking.VersionSync;
using CorgEng.UtilityTypes.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Networking.EntitySystems
{
    public class NetworkSystem : EntitySystem
    {

        [UsingDependency]
        private static IServerCommunicator ServerCommunicator;

        [UsingDependency]
        private static INetworkMessageFactory NetworkMessageFactory;

        public override EntitySystemFlags SystemFlags { get; } = EntitySystemFlags.HOST_SYSTEM;

        public override void SystemSetup()
        {
            RegisterLocalEvent<NetworkTransformComponent, ComponentAddedEvent>(OnComponentAdded);
            RegisterLocalEvent<NetworkTransformComponent, NetworkedEventRaisedEvent>(OnNetworkedEventRaised);
            RegisterGlobalEvent<NetworkedEventRaisedEvent>(OnGlobalNetworkedEventRaised);
        }

        /// <summary>
        /// Called when a component is added to a specified entity.
        /// This will be networked to all clients in view of the entity.
        /// </summary>
        private void OnComponentAdded(Entity entity, NetworkTransformComponent transformComponent, ComponentAddedEvent componentAddedEvent)
        {

        }

        /// <summary>
        /// Called when a global event is raised. Networks that event to all clients.
        /// </summary>
        private void OnGlobalNetworkedEventRaised(NetworkedEventRaisedEvent networkedEventRaisedEvent)
        {
            ServerCommunicator?.SendToClients(
                NetworkMessageFactory.CreateMessage(PacketHeaders.GLOBAL_EVENT_RAISED, InjectEventCode(networkedEventRaisedEvent.RaisedEvent))
                );
        }

        /// <summary>
        /// Triggered when a networked event was raised.
        /// Uses the transform component to determine if the entity is near a camera,
        /// so any networked events need to has a transform component on the entity.
        /// </summary>
        private void OnNetworkedEventRaised(Entity entity, NetworkTransformComponent transformComponent, NetworkedEventRaisedEvent networkedEventRaisedEvent)
        {
            ServerCommunicator?.SendToReleventClients(
                NetworkMessageFactory.CreateMessage(PacketHeaders.LOCAL_EVENT_RAISED, InjectEventCode(networkedEventRaisedEvent.RaisedEvent)),
                transformComponent.Position,
                new Vector<float>(1, 1, 1)
                );
        }

        //Kind of slow due to a lot of memory allocation :(
        private byte[] InjectEventCode(Event e)
        {
            byte[] data = e.Serialize();
            byte[] output = new byte[data.Length + 2];
            BitConverter.GetBytes(e.GetNetworkedIdentifier()).CopyTo(output, 0);
            data.CopyTo(output, 2);
            return output;
        }

    }
}

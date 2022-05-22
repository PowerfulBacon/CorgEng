using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events;
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
    public class NetworkSystem : EntitySystem
    {

        [UsingDependency]
        private static IServerCommunicator ServerCommunicator;

        [UsingDependency]
        private static INetworkMessageFactory NetworkMessageFactory;

        public override void SystemSetup()
        {
            RegisterLocalEvent<TransformComponent, NetworkedEventRaisedEvent>(OnNetworkedEventRaised);
            RegisterGlobalEvent<NetworkedEventRaisedEvent>(OnGlobalNetworkedEventRaised);
        }

        private void OnGlobalNetworkedEventRaised(NetworkedEventRaisedEvent networkedEventRaisedEvent)
        {
            ServerCommunicator?.SendToClients(
                NetworkMessageFactory.CreateMessage(PacketHeaders.EVENT_RAISED, InjectEventCode(networkedEventRaisedEvent.RaisedEvent))
                );
        }

        /// <summary>
        /// Triggered when a networked event was raised.
        /// Uses the transform component to determine if the entity is near a camera,
        /// so any networked events need to has a transform component on the entity.
        /// </summary>
        private void OnNetworkedEventRaised(Entity entity, TransformComponent transformComponent, NetworkedEventRaisedEvent networkedEventRaisedEvent)
        {
            ServerCommunicator?.SendToReleventClients(
                NetworkMessageFactory.CreateMessage(PacketHeaders.EVENT_RAISED, InjectEventCode(networkedEventRaisedEvent.RaisedEvent)),
                transformComponent.Position,
                new Vector<float>(1, 1, 1)
                );
        }

        //Kind of slow due to a lot of memory allocation :(
        private byte[] InjectEventCode(Event e)
        {
            byte[] data = e.Serialize();
            byte[] output = new byte[data.Length + 4];
            BitConverter.GetBytes(e.GetNetworkedID()).CopyTo(output, 0);
            data.CopyTo(output, 4);
            return output;
        }

    }
}

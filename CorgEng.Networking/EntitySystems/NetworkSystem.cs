using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.EntityComponentSystem.Implementations.Transform;
using CorgEng.EntityComponentSystem.Systems;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Networking.Clients;
using CorgEng.GenericInterfaces.Networking.Config;
using CorgEng.GenericInterfaces.Networking.Networking;
using CorgEng.GenericInterfaces.Networking.Networking.Client;
using CorgEng.GenericInterfaces.Networking.Networking.Server;
using CorgEng.GenericInterfaces.Networking.Packets;
using CorgEng.Networking.Components;
using CorgEng.Networking.Networking.Server;
using CorgEng.Networking.VersionSync;
using CorgEng.UtilityTypes.Vectors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Networking.EntitySystems
{
    public class NetworkSystem : EntitySystem
    {

        [UsingDependency]
        private static INetworkConfig NetworkConfig;

        [UsingDependency]
        private static IServerCommunicator ServerCommunicator;

        [UsingDependency]
        private static IClientCommunicator ClientCommunicator;

        [UsingDependency]
        private static INetworkMessageFactory NetworkMessageFactory;

        [UsingDependency]
        private static IEntityCommunicator EntityCommunicator;

        [UsingDependency]
        private static INetworkingServer NetworkingServer;

        public override EntitySystemFlags SystemFlags { get; } = EntitySystemFlags.HOST_SYSTEM | EntitySystemFlags.CLIENT_SYSTEM;

        public override void SystemSetup()
        {
            RegisterLocalEvent<NetworkTransformComponent, ComponentAddedEvent>(OnComponentAdded);
            RegisterLocalEvent<NetworkTransformComponent, NetworkedEventRaisedEvent>(OnNetworkedEventRaised);
            RegisterLocalEvent<NetworkTransformComponent, InitialiseEvent>((e, c, s) => {
                foreach (IClient client in ((NetworkingServer)NetworkingServer).connectedClients.Values)
                    EntityCommunicator.CommunicateEntity(e, client);
            });
            RegisterGlobalEvent<NetworkedEventRaisedEvent>(OnGlobalNetworkedEventRaised);
        }

        /// <summary>
        /// Called when a component is added to a specified entity.
        /// This will be networked to all clients in view of the entity.
        /// </summary>
        private void OnComponentAdded(IEntity entity, NetworkTransformComponent transformComponent, ComponentAddedEvent componentAddedEvent)
        {

        }

        /// <summary>
        /// Called when a global event is raised. Networks that event to all clients.
        /// </summary>
        private void OnGlobalNetworkedEventRaised(NetworkedEventRaisedEvent networkedEventRaisedEvent)
        {
            if (NetworkConfig.ProcessServerSystems)
            {
                //Send the message to clients
                ServerCommunicator?.SendToClients(
                    NetworkMessageFactory.CreateMessage(PacketHeaders.GLOBAL_EVENT_RAISED, InjectEventCode(networkedEventRaisedEvent.RaisedEvent, null))
                    );
            }
            else if (NetworkConfig.ProcessClientSystems)
            {
                //Send the message to the server
                ClientCommunicator?.SendToServer(
                    NetworkMessageFactory.CreateMessage(PacketHeaders.GLOBAL_EVENT_RAISED, InjectEventCode(networkedEventRaisedEvent.RaisedEvent, null))
                    );
            }
            
        }

        /// <summary>
        /// Triggered when a networked event was raised.
        /// Uses the transform component to determine if the entity is near a camera,
        /// so any networked events need to has a transform component on the entity.
        /// </summary>
        private void OnNetworkedEventRaised(IEntity entity, NetworkTransformComponent transformComponent, NetworkedEventRaisedEvent networkedEventRaisedEvent)
        {
            if (NetworkConfig.ProcessServerSystems)
            {
                ServerCommunicator?.SendToReleventClients(
                    NetworkMessageFactory.CreateMessage(PacketHeaders.LOCAL_EVENT_RAISED, InjectEventCode(networkedEventRaisedEvent.RaisedEvent, entity)),
                    transformComponent.Position.Value,
                    new Vector<float>(1, 1, 1)
                    );
            }
        }

        //Kind of slow due to a lot of memory allocation :(
        //TODO: Improve the speed of this by using MemoryStream and BinaryWriter rather than array copies.
        private byte[] InjectEventCode(INetworkedEvent e, IEntity entityTarget)
        {
            //Calculate the serilised length
            int serialisationLength = e.SerialisedLength() + sizeof(int) + sizeof(ushort) + (entityTarget != null ? sizeof(uint) : 0);
            //Create the output byte array
            byte[] outputMemory = new byte[serialisationLength];
            //Begin writing
            using (MemoryStream memoryStream = new MemoryStream(outputMemory))
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
                {
                    binaryWriter.Write(serialisationLength);
                    binaryWriter.Write(e.GetNetworkedIdentifier());
                    if (entityTarget != null)
                    {
                        binaryWriter.Write(entityTarget.Identifier);
                    }
                    e.Serialise(binaryWriter);
                }
            }
            return outputMemory;
        }

    }
}

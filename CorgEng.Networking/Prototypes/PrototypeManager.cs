using CorgEng.Core.Dependencies;
using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Networking.Networking;
using CorgEng.GenericInterfaces.Networking.Networking.Client;
using CorgEng.GenericInterfaces.Networking.Packets;
using CorgEng.GenericInterfaces.Networking.PrototypeManager;
using CorgEng.GenericInterfaces.UtilityTypes.BinaryLists;
using CorgEng.Networking.Components;
using CorgEng.Networking.Exceptions;
using CorgEng.Networking.VersionSync;
using CorgEng.UtilityTypes.Trees;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CorgEng.Networking.Prototypes
{

    [Dependency]
    internal class PrototypeManager : IPrototypeManager
    {

        private class UniqueComponentIdentification
        {
            public int componentIdentifier;
            public IList<long> propertyIdentifier;
        }

        [UsingDependency]
        private static ILogger Logger;

        [UsingDependency]
        private static IBinaryListFactory BinaryListFactory;

        [UsingDependency]
        private static IServerCommunicator ServerCommunicator;

        [UsingDependency]
        private static IClientCommunicator ClientCommunicator;

        [UsingDependency]
        private static INetworkMessageFactory NetworkMessageFactory;

        //We use a tree structure for the prototype fetching (Component IDs need to be in order)
        private TreeNode<long, IPrototype> PrototypeTree = new TreeNode<long, IPrototype>();

        internal static ConcurrentDictionary<uint, IPrototype> PrototypeLookup = new ConcurrentDictionary<uint, IPrototype>();

        /// <summary>
        /// Gets the prototype related to an entity.
        /// This code is pretty long, this probably needs caching or it might be worse
        /// than just sending the whole entity every time.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public IPrototype GetPrototype(IEntity entity, bool network = true)
        {
            //Get a list of all prototyped properties on the component
            IBinaryList<UniqueComponentIdentification> componentIdentifiers = BinaryListFactory.CreateEmpty<UniqueComponentIdentification>();
            //TODO Poor concurrency support
            for (int i = entity.Components.Count - 1; i >= 0; i--)
            {
                //Get the component
                IComponent component = entity.Components[i];
                if (!component.IsSynced)
                    continue;
                //Get the component type
                Type componentType = component.GetType();
                //Get the property info related with this type
                if (!ComponentExtensions.propertyInfoCache.ContainsKey(componentType))
                    continue;
                //Get the component sycned ID
                UniqueComponentIdentification uniqueComponentIdentification = new UniqueComponentIdentification();
                uniqueComponentIdentification.componentIdentifier = component.GetNetworkedIdentifier();
                uniqueComponentIdentification.propertyIdentifier = new List<long>();
                componentIdentifiers.Add(uniqueComponentIdentification.componentIdentifier, uniqueComponentIdentification);
                IEnumerable<(bool, PropertyInfo)> componentTypes = ComponentExtensions.propertyInfoCache[componentType];
                foreach ((bool, PropertyInfo) property in componentTypes)
                {
                    if (!property.Item1)
                        continue;
                    long value = property.Item2.GetValue(component)?.GetHashCode() ?? -1;
                    uniqueComponentIdentification.propertyIdentifier.Add(value);
                }
            }
            //Now we need to traverse the prototype tree to see if it already exists
            UniqueComponentIdentification componentIdentification;
            TreeNode<long, IPrototype> current = PrototypeTree;
            int a = componentIdentifiers.Length();
            while (componentIdentifiers.Length() > 0)
            {
                //TODO: This stops the loop for some reason
                componentIdentification = componentIdentifiers.TakeFirst();
                //First go to the component identifier
                current = current.GotoChildOrCreate(componentIdentification.componentIdentifier);
                //Then go to the property identifiers
                foreach (long componentValue in componentIdentification.propertyIdentifier)
                {
                    current = current.GotoChildOrCreate(componentValue);
                }
            }
            //The current node is now the prototype we require
            if (current.Value != null)
                return current.Value;
            //Create the prototype and tell all clients about the new prototype
            IPrototype createdPrototype = new Prototype();
            createdPrototype.GenerateFromEntity(entity);
            //Add the prototype to the prototype lookup table
            PrototypeLookup.TryAdd(createdPrototype.Identifier, createdPrototype);
            INetworkMessage message = NetworkMessageFactory.CreateMessage(
                PacketHeaders.PROTOTYPE_INFO,
                createdPrototype.SerializePrototype()
                );
            if(network)
                ServerCommunicator.SendToClients(message);
            return createdPrototype;
        }

        public IPrototype GetPrototype(byte[] serializedPrototype)
        {
            IPrototype prototype = new Prototype();
            prototype.DeserializePrototype(serializedPrototype);
            return prototype;
        }

        /// <summary>
        /// Anything that needs to be called when we recieve a new prototype.
        /// </summary>
        private ConcurrentDictionary<Action<IPrototype>, bool> prototypeAddCallbacks = new ConcurrentDictionary<Action<IPrototype>, bool>();

        public void AddPrototype(IPrototype prototype)
        {
            Logger.WriteLine($"Prototype {prototype.Identifier} created successfully.", LogType.DEBUG);
            PrototypeLookup.AddOrUpdate(prototype.Identifier, prototype, (key, value) => prototype);
            //Call the prototype added trigger
            foreach (KeyValuePair<Action<IPrototype>, bool> prototypeAddCallback in prototypeAddCallbacks)
            {
                prototypeAddCallback.Key(prototype);
            }
        }

        public Task<IPrototype> GetPrototype(uint prototypeIdentifier)
        {
            if (PrototypeLookup.ContainsKey(prototypeIdentifier))
            {
                return Task.FromResult(PrototypeLookup[prototypeIdentifier]);
            }
            IPrototype located = null;
            bool successStateAchieved = false;
            AutoResetEvent waitEvent = new AutoResetEvent(false);
            //Add our callback for success
            Action<IPrototype> createdCallback = null;
            createdCallback = prototype => {
                if (prototype.Identifier != prototypeIdentifier)
                    return;
                //Remove ourself from the dictionary.
                successStateAchieved = true;
                prototypeAddCallbacks.TryRemove(createdCallback, out _);
                located = prototype;
                waitEvent.Set();
            };
            prototypeAddCallbacks.TryAdd(createdCallback, true);
            //Ask the server for the prototype we want
            ClientCommunicator?.SendToServer(NetworkMessageFactory.CreateMessage(
                PacketHeaders.REQUEST_PROTOTYPE,
                BitConverter.GetBytes(prototypeIdentifier)
                ));
            Logger.WriteLine($"Requesting prototype {prototypeIdentifier} from server...", LogType.DEBUG);
            //Wait until we recieve the requested prototype. Send the request every 100ms until we get a result
            //TODO: After 2 seconds, timeout and disconnect from the server
            int attemptsRemaining = 20;
            while (waitEvent.WaitOne(150) && attemptsRemaining-- > 0)
            {
                //We were successful
                if (successStateAchieved)
                {
                    Logger.WriteLine($"Prototype {prototypeIdentifier} successfully retrieved from server!", LogType.DEBUG);
                    return Task.FromResult(located);
                }
                //Re-request
                Logger.WriteLine($"Requesting prototype {prototypeIdentifier} from server...", LogType.DEBUG);
                ClientCommunicator?.SendToServer(NetworkMessageFactory.CreateMessage(
                    PacketHeaders.REQUEST_PROTOTYPE,
                    BitConverter.GetBytes(prototypeIdentifier)
                    ));
            }
            Logger.WriteLine($"Failed to fetch prototype {prototypeIdentifier} from server after 10 attempts.", LogType.WARNING);
            throw new NetworkingException($"Failed to fetch prototype {prototypeIdentifier} from server, server is not responding.");
        }

        /// <summary>
        /// Get a prototype that is currently saved.
        /// Intended for the server.
        /// </summary>
        /// <param name="prototypeIdentifier"></param>
        /// <returns></returns>
        public IPrototype GetLocalProtoype(uint prototypeIdentifier)
        {
            if (PrototypeLookup.ContainsKey(prototypeIdentifier))
            {
                return PrototypeLookup[prototypeIdentifier];
            }
            return null;
        }

    }
}

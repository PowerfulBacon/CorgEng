using CorgEng.Core.Dependencies;
using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Networking.Networking;
using CorgEng.GenericInterfaces.Networking.Packets;
using CorgEng.GenericInterfaces.Networking.PrototypeManager;
using CorgEng.GenericInterfaces.UtilityTypes.BinaryLists;
using CorgEng.Networking.Components;
using CorgEng.Networking.VersionSync;
using CorgEng.UtilityTypes.Trees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Networking.Prototypes
{

    [Dependency]
    internal class PrototypeManager : IPrototypeManager
    {

        private struct UniqueComponentIdentification
        {
            public int componentIdentifier;
            public IList<long> propertyIdentifier;
        }

        [UsingDependency]
        private static IBinaryListFactory BinaryListFactory;

        [UsingDependency]
        private static IServerCommunicator ServerCommunicator;

        [UsingDependency]
        private static INetworkMessageFactory NetworkMessageFactory;

        //We use a tree structure for the prototype fetching (Component IDs need to be in order)
        private TreeNode<long, IPrototype> PrototypeTree = new TreeNode<long, IPrototype>();

        /// <summary>
        /// Gets the prototype related to an entity.
        /// This code is pretty long, this probably needs caching or it might be worse
        /// than just sending the whole entity every time.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public IPrototype GetPrototype(IEntity entity)
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
                IEnumerable<PropertyInfo> componentTypes = ComponentExtensions.propertyInfoCache[componentType];
                foreach (PropertyInfo property in componentTypes)
                {
                    long value = property.GetValue(component).GetHashCode();
                    uniqueComponentIdentification.propertyIdentifier.Add(value);
                }
            }
            //Now we need to traverse the prototype tree to see if it already exists
            UniqueComponentIdentification componentIdentification;
            TreeNode<long, IPrototype> current = PrototypeTree;
            while (componentIdentifiers.Length() > 0)
            {
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
            INetworkMessage message = NetworkMessageFactory.CreateMessage(
                PacketHeaders.PROTOTYPE_INFO,

                );
            ServerCommunicator.SendToClients();
            return createdPrototype;
        }

        public IPrototype GetProtoype(byte[] serializedPrototype)
        {
            IPrototype prototype = new Prototype();
            prototype.DeserializePrototype(serializedPrototype);
            return prototype;
        }
    }
}

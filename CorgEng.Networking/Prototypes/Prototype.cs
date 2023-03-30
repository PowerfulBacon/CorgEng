using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.EntityComponentSystem.Implementations.Deletion;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Networking.PrototypeManager;
using CorgEng.GenericInterfaces.Networking.Serialisation;
using CorgEng.GenericInterfaces.Serialization;
using CorgEng.Networking.Components;
using CorgEng.Networking.Serialization;
using CorgEng.Networking.VersionSync;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Networking.Prototypes
{
    internal class Prototype : IPrototype
    {

        [UsingDependency]
        private static ILogger Logger;

        [UsingDependency]
        private static IAutoSerialiser AutoSerialiser;

        [UsingDependency]
        private static IPrototypeManager PrototypeManager;

        private static uint PrototypeIdentifierHighest = 0;

        public uint Identifier { get; set; } = PrototypeIdentifierHighest++;

        /// <summary>
        /// The prototype components
        /// </summary>
        Dictionary<Type, Dictionary<PropertyInfo, object>> prototypeComponents = new Dictionary<Type, Dictionary<PropertyInfo, object>>();

        public void GenerateFromEntity(IEntity entity)
        {
            foreach (IComponent component in entity.Components)
            {
                //Skip deletable component
                if (component is DeleteableComponent)
                    continue;
                Dictionary<PropertyInfo, object> propertyVariableInformation = new Dictionary<PropertyInfo, object>();
                //Load the property information
                foreach ((bool, PropertyInfo) propertyInfo in ComponentExtensions.propertyInfoCache[component.GetType()])
                {
                    if (!propertyInfo.Item1)
                        continue;
                    propertyVariableInformation.Add(propertyInfo.Item2, propertyInfo.Item2.GetValue(component));
                }
                //Store
                prototypeComponents.Add(component.GetType(), propertyVariableInformation);
            }
        }

        public IEntity CreateEntityFromPrototype(IWorld world)
        {
            return CreateEntityFromPrototype(world, world.EntityManager.GetNewEntityId());
        }

        public IEntity CreateEntityFromPrototype(IWorld world, uint entityIdentifier)
        {
            IEntity createdEntity = world.EntityManager.CreateUninitialisedEntity(entityIdentifier);
            //Add components
            foreach (Type type in prototypeComponents.Keys)
            {
                //Create the uninitialized component
                IComponent createdComponent = (IComponent)Activator.CreateInstance(type);
                //Inject variables
                foreach (PropertyInfo propertyInfo in prototypeComponents[type].Keys)
                {
                    propertyInfo.SetValue(createdComponent, prototypeComponents[type][propertyInfo]);
                }
                //Add the component
                createdEntity.AddComponent(createdComponent);
            }
            //Raise the initialise event
            new InitialiseEvent().Raise(createdEntity, true);
            return createdEntity;
        }

        public void DeserializePrototype(byte[] data)
        {
            using (MemoryStream memStream = new MemoryStream(data))
            {
                using (BinaryReader binaryReader = new BinaryReader(memStream))
                {
                    //Read the prototype identifier
                    Identifier = binaryReader.ReadUInt32();
                    //Read all components
                    while (binaryReader.PeekChar() != -1)
                    {
                        //Read the component type identifier
                        ushort componentTypeIdentifier = binaryReader.ReadUInt16();
                        //Create an uninitialised version of that component
                        IComponent uninitialisedComponent = VersionGenerator.CreateTypeFromIdentifier<IComponent>(componentTypeIdentifier);
                        Dictionary<PropertyInfo, object> variableProperties = new Dictionary<PropertyInfo, object>();
                        //Go ahead and set all the properties of the component
                        foreach ((bool, PropertyInfo) propInfo in ComponentExtensions.propertyInfoCache[uninitialisedComponent.GetType()])
                        {
                            if (!propInfo.Item1)
                                continue;
                            variableProperties.Add(propInfo.Item2, AutoSerialiser.Deserialize(propInfo.Item2.PropertyType, binaryReader));
                        }
                        Type type = VersionGenerator.GetTypeFromNetworkedIdentifier(componentTypeIdentifier);
                        if (prototypeComponents.ContainsKey(type))
                        {
                            //TODO: I just found this conditional with a semicolon in it, I have no idea what it does or what it is meant to do to be honest
                            //I think it was a temporary thing to catch error cases?
                            ;
                        }
                        //Add the component to the property
                        prototypeComponents.Add(type, variableProperties);
                    }
                    //Add to the lookup table
                    PrototypeManager.AddPrototype(this);
                }
            }
        }

        public byte[] SerializePrototype()
        {
            List<object> objectsToWrite = new List<object>();
            List<Type> typesToWrite = new List<Type>(); //Required for null handling
            //Write the prototype identifier
            int size = 0;
            objectsToWrite.Add(Identifier);
            typesToWrite.Add(typeof(uint));
            size += sizeof(uint);
            //Go through each component and serialize it
            foreach (Type componentType in prototypeComponents.Keys)
            {
                //Ignore deletable compoent since its common
                if (componentType == typeof(DeleteableComponent))
                    continue;
                //First serialize a component identifier
                ushort typeIdentifier = componentType.GetNetworkedIdentifier();
                size += sizeof(ushort);
                objectsToWrite.Add(typeIdentifier);
                typesToWrite.Add(typeof(ushort));
                //Write the component data
                foreach ((bool, PropertyInfo) propInfo in ComponentExtensions.propertyInfoCache[componentType])
                {
                    if (!propInfo.Item1)
                        continue;
                    object valueToWrite = prototypeComponents[componentType][propInfo.Item2];
                    objectsToWrite.Add(valueToWrite);
                    typesToWrite.Add(propInfo.Item2.PropertyType);
                    size += AutoSerialiser.SerialisationLength(propInfo.Item2.PropertyType, valueToWrite);
                }
            }
            //Begin the writing process
            byte[] serializedArray = new byte[size];
            using (MemoryStream memStream = new MemoryStream(serializedArray))
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(memStream))
                {
                    for (int i = 0; i < objectsToWrite.Count; i++)
                    {
                        AutoSerialiser.SerializeInto(typesToWrite[i], objectsToWrite[i], binaryWriter);
                    }
                }
            }
            return serializedArray;
        }

    }
}

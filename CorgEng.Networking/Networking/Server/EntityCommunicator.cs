using CorgEng.Core.Dependencies;
using CorgEng.DependencyInjection.Dependencies;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Networking.Clients;
using CorgEng.GenericInterfaces.Networking.Networking.Server;
using CorgEng.GenericInterfaces.Networking.PrototypeManager;
using CorgEng.GenericInterfaces.Serialization;
using CorgEng.Networking.Components;
using CorgEng.Networking.VersionSync;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Networking.Networking.Server
{
    [Dependency]
    internal class EntityCommunicator : IEntityCommunicator
    {

        [UsingDependency]
        private static IPrototypeManager PrototypeManager;

        [UsingDependency]
        private static IAutoSerialiser AutoSerialiser;

        /// <summary>
        /// Communicate information about an entity to a client.
        /// We need to include:
        /// - Prototype ID
        /// - Any non-prototyped variables
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="target"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void CommunicateEntity(IEntity entity, IClient target)
        {
            throw new NotImplementedException();
        }

        public async Task<IEntity> DeserialiseEntity(byte[] data)
        {
            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                using (BinaryReader binaryReader = new BinaryReader(memoryStream))
                {
                    //Get the entity and prototype identifier
                    uint entityIdentifier = binaryReader.ReadUInt32();
                    uint prototypeIdentifier = binaryReader.ReadUInt32();
                    //Create or locate the entity
                    IEntity entity = EntityManager.GetEntity(entityIdentifier);
                    if (entity == null)
                    {
                        //We need to create the entity
                        //Create it based on the prototype we have, which we need to request from the server if we don't have it already
                        IPrototype locatedPrototype = await PrototypeManager.GetPrototype(prototypeIdentifier);
                        //Create the entity with a specific identifier
                        entity = locatedPrototype.CreateEntityFromPrototype(entityIdentifier);
                    }
                    //Deserialise component variables
                    foreach (IComponent component in entity.Components)
                    {
                        //Get the component variables
                        IEnumerable<(bool, PropertyInfo)> componentVariables = ComponentExtensions.propertyInfoCache[component.GetType()];
                        //Locate all the ones that aren't prototyped
                        foreach ((bool, PropertyInfo) componentVariable in componentVariables)
                        {
                            //Skip prototyped variables
                            if (componentVariable.Item1)
                                continue;
                            //Add to the serialisation
                            object value = AutoSerialiser.Deserialize(componentVariable.Item2.PropertyType, binaryReader);
                            componentVariable.Item2.SetValue(component, value);
                        }
                    }
                    //Return the created entity
                    return entity;
                }
            }
        }

        public byte[] SerializeEntity(IEntity entity)
        {
            //Determine the size
            int sizeRequired = sizeof(uint) + sizeof(uint);
            //Things we need to serialise
            IList<object> thingsToSerialise = new List<object>();
            thingsToSerialise.Add(entity.Identifier);
            thingsToSerialise.Add(PrototypeManager.GetPrototype(entity).Identifier);
            //Add all variables that require serialisation
            foreach (IComponent component in entity.Components)
            {
                //Get the component variables
                IEnumerable<(bool, PropertyInfo)> componentVariables = ComponentExtensions.propertyInfoCache[component.GetType()];
                //Locate all the ones that aren't prototyped
                foreach ((bool, PropertyInfo) componentVariable in componentVariables)
                {
                    //Skip prototyped variables
                    if (componentVariable.Item1)
                        continue;
                    //Add to the serialisation
                    sizeRequired += AutoSerialiser.SerialisationLength(componentVariable.Item2.GetValue(component));
                    thingsToSerialise.Add(componentVariable.Item2.GetValue(component));
                }
            }
            //Add all the things
            byte[] byteArray = new byte[sizeRequired];
            using (MemoryStream memStream = new MemoryStream(byteArray))
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(memStream))
                {
                    foreach (object thing in thingsToSerialise)
                    {
                        AutoSerialiser.SerializeInto(thing, binaryWriter);
                    }
                }
            }
            //Return the created value
            return byteArray;
        }

    }
}

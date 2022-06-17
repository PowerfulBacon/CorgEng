using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Networking.PrototypeManager;
using CorgEng.GenericInterfaces.Networking.Serialisation;
using CorgEng.Networking.Components;
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
                Dictionary<PropertyInfo, object> propertyVariableInformation = new Dictionary<PropertyInfo, object>();
                //Load the property information
                foreach (PropertyInfo propertyInfo in ComponentExtensions.propertyInfoCache[component.GetType()])
                {
                    propertyVariableInformation.Add(propertyInfo, propertyInfo.GetValue(component));
                }
                //Store
                prototypeComponents.Add(component.GetType(), propertyVariableInformation);
            }
        }

        public IEntity CreateEntityFromPrototype()
        {
            IEntity createdEntity = new Entity();
            //Add components
            foreach (Type type in prototypeComponents.Keys)
            {
                //Create the uninitialized component
                IComponent createdComponent = (IComponent)FormatterServices.GetUninitializedObject(type);
                //Inject variables
                foreach (PropertyInfo propertyInfo in prototypeComponents[type].Keys)
                {
                    propertyInfo.SetValue(createdComponent, prototypeComponents[type][propertyInfo]);
                }
                //Add the component
                createdEntity.AddComponent(createdComponent);
            }
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
                        foreach (PropertyInfo propInfo in ComponentExtensions.propertyInfoCache[uninitialisedComponent.GetType()])
                        {
                            //Set the value based on the property type
                            if (propInfo.PropertyType == typeof(string))
                            {
                                ushort stringLength = binaryReader.ReadUInt16();
                                byte[] byteArray = binaryReader.ReadBytes(stringLength);
                                variableProperties.Add(propInfo, Encoding.ASCII.GetString(byteArray));
                            }
                            else if (propInfo.PropertyType == typeof(byte))
                                variableProperties.Add(propInfo, binaryReader.ReadByte());
                            else if (propInfo.PropertyType == typeof(char))
                                variableProperties.Add(propInfo, binaryReader.ReadChar());
                            else if (propInfo.PropertyType == typeof(int))
                                variableProperties.Add(propInfo, binaryReader.ReadInt32());
                            else if (propInfo.PropertyType == typeof(float))
                                variableProperties.Add(propInfo, binaryReader.ReadSingle());
                            else if (propInfo.PropertyType == typeof(double))
                                variableProperties.Add(propInfo, binaryReader.ReadDouble());
                            else if (propInfo.PropertyType == typeof(long))
                                variableProperties.Add(propInfo, binaryReader.ReadInt64());
                            else if (propInfo.PropertyType == typeof(short))
                                variableProperties.Add(propInfo, binaryReader.ReadInt16());
                            else if (propInfo.PropertyType == typeof(uint))
                                variableProperties.Add(propInfo, binaryReader.ReadUInt32());
                            else if (propInfo.PropertyType == typeof(ushort))
                                variableProperties.Add(propInfo, binaryReader.ReadUInt16());
                            else if (propInfo.PropertyType == typeof(ulong))
                                variableProperties.Add(propInfo, binaryReader.ReadUInt64());
                            else if (propInfo.PropertyType == typeof(decimal))
                                variableProperties.Add(propInfo, binaryReader.ReadDecimal());
                            else
                                binaryReader.ReadBytes(Marshal.SizeOf(propInfo.PropertyType));
                        }
                        //Add the component to the property
                        prototypeComponents.Add(VersionGenerator.GetTypeFromNetworkedIdentifier(componentTypeIdentifier), variableProperties);
                    }
                }
            }
        }

        public byte[] SerializePrototype()
        {
            List<object> objectsToWrite = new List<object>();
            //Write the prototype identifier
            int size = 0;
            objectsToWrite.Add(Identifier);
            size += sizeof(uint);
            //Go through each component and serialize it
            foreach (Type componentType in prototypeComponents.Keys)
            {
                //First serialize a component identifier
                ushort typeIdentifier = componentType.GetNetworkedIdentifier();
                size += sizeof(ushort);
                objectsToWrite.Add(typeIdentifier);
                //Write the component data
                foreach (PropertyInfo propInfo in ComponentExtensions.propertyInfoCache[componentType])
                {
                    object valueToWrite = prototypeComponents[componentType][propInfo];
                    objectsToWrite.Add(valueToWrite);
                    if (valueToWrite is string)
                    {
                        size += sizeof(byte) * ((string)valueToWrite).Length + sizeof(ushort);
                    }
                    else
                    {
                        size += Marshal.SizeOf(valueToWrite);
                    }
                }
            }
            Logger?.WriteLine($"Creating a memory stream with size: {size}", LogType.TEMP);
            //Begin the writing process
            byte[] serializedArray = new byte[size];
            using (MemoryStream memStream = new MemoryStream(serializedArray))
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(memStream))
                {
                    foreach (object objectToWrite in objectsToWrite)
                    {
                        if (typeof(ICustomSerialisationBehaviour).IsAssignableFrom(objectToWrite.GetType()))
                            ((ICustomSerialisationBehaviour)objectToWrite).SerialiseInto(binaryWriter);
                        else if (objectToWrite.GetType() == typeof(string))
                        {
                            byte[] byteArray = Encoding.ASCII.GetBytes(objectToWrite.ToString());
                            binaryWriter.Write((ushort)byteArray.Length);
                            binaryWriter.Write(byteArray);
                        }
                        else if (objectToWrite.GetType() == typeof(byte))
                            binaryWriter.Write((byte)objectToWrite);
                        else if (objectToWrite.GetType() == typeof(char))
                            binaryWriter.Write((char)objectToWrite);
                        else if (objectToWrite.GetType() == typeof(int))
                            binaryWriter.Write((int)objectToWrite);
                        else if (objectToWrite.GetType() == typeof(float))
                            binaryWriter.Write((float)objectToWrite);
                        else if (objectToWrite.GetType() == typeof(double))
                            binaryWriter.Write((double)objectToWrite);
                        else if (objectToWrite.GetType() == typeof(long))
                            binaryWriter.Write((long)objectToWrite);
                        else if (objectToWrite.GetType() == typeof(short))
                            binaryWriter.Write((short)objectToWrite);
                        else if (objectToWrite.GetType() == typeof(uint))
                            binaryWriter.Write((uint)objectToWrite);
                        else if (objectToWrite.GetType() == typeof(ushort))
                            binaryWriter.Write((ushort)objectToWrite);
                        else if (objectToWrite.GetType() == typeof(ulong))
                            binaryWriter.Write((ulong)objectToWrite);
                        else if (objectToWrite.GetType() == typeof(decimal))
                            binaryWriter.Write((decimal)objectToWrite);
                        else
                            binaryWriter.Seek(Marshal.SizeOf(objectToWrite), SeekOrigin.Current);
                    }
                }
            }
            return serializedArray;
        }

    }
}

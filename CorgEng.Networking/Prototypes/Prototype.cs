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
            throw new NotImplementedException();
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
                    size += Marshal.SizeOf(valueToWrite);
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
                            byte[] byteArray = Encoding.ASCII.GetBytes(objectsToWrite.ToString());
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

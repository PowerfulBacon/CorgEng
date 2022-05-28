using CorgEng.Core.Dependencies;
using CorgEng.Core.Modules;
using CorgEng.EntityComponentSystem.Components;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Networking.Attributes;
using CorgEng.GenericInterfaces.Networking.Serialisation;
using CorgEng.UtilityTypes.Vectors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Networking.Components
{
    public static class ComponentExtensions
    {

        [UsingDependency]
        private static ILogger Logger;

        /// <summary>
        /// A cache of the property infos we need to serialize against their type.
        /// Reflection can be incredibly slow, so caching this information is a requirement.
        /// </summary>
        private static Dictionary<Type, IEnumerable<PropertyInfo>> propertyInfoCache = new Dictionary<Type, IEnumerable<PropertyInfo>>();

        /// <summary>
        /// Enumerates through all component types and finds all properties that have
        /// the network serialized tag.
        /// </summary>
        [ModuleLoad]
        public static void LoadPropertyInfoCache()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            Logger?.WriteLine($"Loading Networking Component Extensions...", LogType.LOG);
            //Locate all component types
            IEnumerable<Type> locatedComponentTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(type => typeof(Component).IsAssignableFrom(type) && !type.IsAbstract);
            //Populate the property info cache
            foreach (Type componentType in locatedComponentTypes)
            {
                //Get all fields on this member
                IEnumerable<PropertyInfo> serializedPropertyFields = componentType.GetProperties()
                    .Where(propertyInfo => propertyInfo.GetCustomAttribute<NetworkSerializedAttribute>() != null);
                //Add to the property info cache
                propertyInfoCache.Add(componentType, serializedPropertyFields);
                //Check for serialization errors
                foreach (PropertyInfo propertyInfo in serializedPropertyFields)
                {
                    //Accept strings, despite not being value types
                    if (propertyInfo.PropertyType == typeof(string))
                        continue;
                    //Ensure that we are a value type
                    if (!propertyInfo.PropertyType.IsPrimitive && !typeof(ICustomSerialisationBehaviour).IsAssignableFrom(propertyInfo.PropertyType))
                    {
                        Logger?.WriteLine($"{propertyInfo.PropertyType} is not a value type, serialization errors may occur!", LogType.ERROR);
                        continue;
                    }
                    //Generic struct handling
                    if (propertyInfo.PropertyType.IsGenericType)
                    {
                        if (!propertyInfo.PropertyType.IsPrimitive && propertyInfo.PropertyType != typeof(string) && !typeof(ICustomSerialisationBehaviour).IsAssignableFrom(propertyInfo.PropertyType))
                        {
                            Logger?.WriteLine($"{propertyInfo.PropertyType.GetGenericTypeDefinition()} (Inside of {propertyInfo.PropertyType}) is not a value type, serialization errors may occur!", LogType.ERROR);
                            continue;
                        }
                    }
                }
            }
            //Give confirmation message
            stopwatch.Stop();
            Logger?.WriteLine($"Successfully generated networked component cache for {locatedComponentTypes.Count()} components in {stopwatch.ElapsedMilliseconds}ms.", LogType.LOG);
        }

        /// <summary>
        /// Extension method that takes a byte array and converts it to properties of this
        /// component.
        /// </summary>
        /// <param name="component">The uninitialized component to deserialise the data into</param>
        /// <param name="data">The data to deserialise.</param>
        public static void AutoDeserialize(this Component component, byte[] data)
        {
            IEnumerable<PropertyInfo> targetPropertyInfomation = propertyInfoCache[component.GetType()];
            //Create the binary reader
            BinaryReader binaryReader = new BinaryReader(new MemoryStream(data)); 
            foreach (PropertyInfo propertyInfo in targetPropertyInfomation)
            {

                if (propertyInfo.PropertyType == typeof(ICustomSerialisationBehaviour))
                {
                    ICustomSerialisationBehaviour thing = (ICustomSerialisationBehaviour)FormatterServices.GetUninitializedObject(propertyInfo.PropertyType);
                    thing.DeserialiseFrom(binaryReader);
                    propertyInfo.SetValue(component, thing);
                }
                else if (propertyInfo.PropertyType == typeof(byte[]))
                {
                    ushort length = binaryReader.ReadUInt16();
                    propertyInfo.SetValue(component, binaryReader.ReadBytes(length));
                }
                else if (propertyInfo.PropertyType == typeof(byte))
                    propertyInfo.SetValue(component, binaryReader.ReadByte());
                else if (propertyInfo.PropertyType == typeof(char))
                    propertyInfo.SetValue(component, binaryReader.ReadChar());
                else if (propertyInfo.PropertyType == typeof(int))
                    propertyInfo.SetValue(component, binaryReader.ReadInt32());
                else if (propertyInfo.PropertyType == typeof(float))
                    propertyInfo.SetValue(component, binaryReader.ReadSingle());
                else if (propertyInfo.PropertyType == typeof(double))
                    propertyInfo.SetValue(component, binaryReader.ReadDouble());
                else if (propertyInfo.PropertyType == typeof(long))
                    propertyInfo.SetValue(component, binaryReader.ReadInt64());
                else if (propertyInfo.PropertyType == typeof(short))
                    propertyInfo.SetValue(component, binaryReader.ReadInt16());
                else if (propertyInfo.PropertyType == typeof(uint))
                    propertyInfo.SetValue(component, binaryReader.ReadUInt32());
                else if (propertyInfo.PropertyType == typeof(ushort))
                    propertyInfo.SetValue(component, binaryReader.ReadUInt16());
                else if (propertyInfo.PropertyType == typeof(ulong))
                    propertyInfo.SetValue(component, binaryReader.ReadUInt64());
                else if (propertyInfo.PropertyType == typeof(decimal))
                    propertyInfo.SetValue(component, binaryReader.ReadDecimal());
                else
                    binaryReader.ReadBytes(Marshal.SizeOf(propertyInfo.PropertyType));
            }
        }

        /// <summary>
        /// Extension method for component, provides automatic serialization
        /// for commonly used types.
        /// This is such a mess.
        /// </summary>
        /// <returns>Returns a byte array representing the component serialized.</returns>
        public static byte[] AutoSerialize(this Component component)
        {
            IEnumerable<PropertyInfo> targetPropertyInfomation = propertyInfoCache[component.GetType()];
            List<object> values = new List<object>();
            //Determine the length of what we are serializing
            int requiredLength = 0;
            foreach (PropertyInfo propertyInfo in targetPropertyInfomation)
            {
                object propertyValue = propertyInfo.GetValue(component);
                if (propertyValue is ICustomSerialisationBehaviour customSerialisationBehaviour)
                {
                    requiredLength += customSerialisationBehaviour.GetSerialisationLength();
                }
                //Custom behaviour for string: Convert to byte array
                else if (propertyValue is string text)
                {
                    byte[] byteArray = Encoding.ASCII.GetBytes(text);
                    requiredLength += byteArray.Length + 2;
                    values.Add(byteArray);
                    continue;
                }
                else if (propertyInfo.PropertyType.IsPrimitive)
                {
                    requiredLength += Marshal.SizeOf(propertyInfo.PropertyType);
                }
                else
                    continue;
                values.Add(propertyValue);
            }
            //Create the output array
            byte[] serializedComponent = new byte[requiredLength];
            //Create a memory stream to write into the array
            MemoryStream memoryStream = new MemoryStream(serializedComponent);
            //Create a binary writer, to write into the array
            BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            foreach (object value in values)
            {
                if (value is ICustomSerialisationBehaviour customSerialisationBehaviour)
                    customSerialisationBehaviour.SerialiseInto(binaryWriter);
                else if (value is byte[] byteArray)
                {
                    binaryWriter.Write((ushort)byteArray.Length);
                    binaryWriter.Write(byteArray);
                }
                else if (value is byte valueByte)
                    binaryWriter.Write(valueByte);
                else if (value is char valueChar)
                    binaryWriter.Write(valueChar);
                else if (value is int valueInt)
                    binaryWriter.Write(valueInt);
                else if (value is float valueFloat)
                    binaryWriter.Write(valueFloat);
                else if (value is double valueDouble)
                    binaryWriter.Write(valueDouble);
                else if (value is long valueLong)
                    binaryWriter.Write(valueLong);
                else if (value is short valueShort)
                    binaryWriter.Write(valueShort);
                else if (value is uint valueUint)
                    binaryWriter.Write(valueUint);
                else if (value is ushort valueUshort)
                    binaryWriter.Write(valueUshort);
                else if (value is ulong valueUlong)
                    binaryWriter.Write(valueUlong);
                else if (value is decimal valueDecimal)
                    binaryWriter.Write(valueDecimal);
                else
                    binaryWriter.Seek(Marshal.SizeOf(value), SeekOrigin.Current);
            }
            binaryWriter.Close();
            //Write the data
            return serializedComponent;
        }

    }
}

using CorgEng.Core.Dependencies;
using CorgEng.Core.Modules;
using CorgEng.EntityComponentSystem.Components;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Networking.Attributes;
using CorgEng.GenericInterfaces.Networking.Serialisation;
using CorgEng.GenericInterfaces.Serialization;
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

        [UsingDependency]
        private static IAutoSerialiser AutoSerialiser;

        /// <summary>
        /// A cache of the property infos we need to serialize against their type.
        /// Reflection can be incredibly slow, so caching this information is a requirement.
        /// bool - Is this prototyped.
        /// </summary>
        internal static Dictionary<Type, IEnumerable<(bool, PropertyInfo)>> propertyInfoCache = new Dictionary<Type, IEnumerable<(bool, PropertyInfo)>>();

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
                IEnumerable<(bool, PropertyInfo)> serializedPropertyFields = componentType.GetProperties()
                    .Where(propertyInfo => propertyInfo.GetCustomAttribute<NetworkSerializedAttribute>() != null)
                    .Select(propertyInfo => (propertyInfo.GetCustomAttribute<NetworkSerializedAttribute>().prototypeInclude, propertyInfo));
                //Add to the property info cache
                propertyInfoCache.Add(componentType, serializedPropertyFields);
                //Check for serialization errors
                foreach ((bool, PropertyInfo) propertyInfo in serializedPropertyFields)
                {
                    //Accept strings, despite not being value types
                    if (propertyInfo.Item2.PropertyType == typeof(string))
                        continue;
                    //Ensure that we are a value type
                    if (!propertyInfo.Item2.PropertyType.IsPrimitive && !typeof(ICustomSerialisationBehaviour).IsAssignableFrom(propertyInfo.Item2.PropertyType))
                    {
                        Logger?.WriteLine($"{propertyInfo.Item2.PropertyType} is not a value type, serialization errors may occur!", LogType.ERROR);
                        continue;
                    }
                    //Generic struct handling
                    if (propertyInfo.Item2.PropertyType.IsGenericType)
                    {
                        if (!propertyInfo.Item2.PropertyType.IsPrimitive && propertyInfo.Item2.PropertyType != typeof(string) && !typeof(ICustomSerialisationBehaviour).IsAssignableFrom(propertyInfo.Item2.PropertyType))
                        {
                            Logger?.WriteLine($"{propertyInfo.Item2.PropertyType.GetGenericTypeDefinition()} (Inside of {propertyInfo.Item2.PropertyType}) is not a value type, serialization errors may occur!", LogType.ERROR);
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
        public static void AutoDeserialize(this Component component, byte[] data, bool isPrototype)
        {
            IEnumerable<(bool, PropertyInfo)> targetPropertyInfomation = propertyInfoCache[component.GetType()];
            //Create the binary reader
            BinaryReader binaryReader = new BinaryReader(new MemoryStream(data)); 
            foreach ((bool, PropertyInfo) propertyInfo in targetPropertyInfomation)
            {
                if (propertyInfo.Item1 != isPrototype)
                    continue;
                object deserialized = AutoSerialiser.Deserialize(propertyInfo.Item2.PropertyType, binaryReader);
                if (deserialized != null)
                {
                    propertyInfo.Item2.SetValue(component, deserialized);
                }
            }
        }

        /// <summary>
        /// Extension method for component, provides automatic serialization
        /// for commonly used types.
        /// This is such a mess.
        /// </summary>
        /// <returns>Returns a byte array representing the component serialized.</returns>
        public static byte[] AutoSerialize(this Component component, bool isPrototype)
        {
            IEnumerable<(bool, PropertyInfo)> targetPropertyInfomation = propertyInfoCache[component.GetType()];
            List<object> values = new List<object>();
            List<Type> types = new List<Type>();
            //Determine the length of what we are serializing
            int requiredLength = 0;
            foreach ((bool, PropertyInfo) propertyInfo in targetPropertyInfomation)
            {
                if (propertyInfo.Item1 != isPrototype)
                    continue;
                object propertyValue = propertyInfo.Item2.GetValue(component);
                requiredLength += AutoSerialiser.SerialisationLength(propertyInfo.Item2.PropertyType, propertyValue);
                values.Add(propertyValue);
                types.Add(propertyInfo.Item2.PropertyType);
            }
            //Create the output array
            byte[] serializedComponent = new byte[requiredLength];
            //Create a memory stream to write into the array
            MemoryStream memoryStream = new MemoryStream(serializedComponent);
            //Create a binary writer, to write into the array
            BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            for (int i = 0; i < values.Count; i++)
            {
                AutoSerialiser.SerializeInto(types[i], values[i], binaryWriter);
            }
            binaryWriter.Close();
            //Write the data
            return serializedComponent;
        }

    }
}

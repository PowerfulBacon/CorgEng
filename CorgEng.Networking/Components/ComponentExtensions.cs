using CorgEng.Core.Dependencies;
using CorgEng.Core.Modules;
using CorgEng.EntityComponentSystem.Components;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Networking.Attributes;
using CorgEng.UtilityTypes.Vectors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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
                    if (!propertyInfo.PropertyType.IsValueType)
                    {
                        Logger?.WriteLine($"{propertyInfo.PropertyType} is not a value type, serialization errors may occur!", LogType.ERROR);
                        continue;
                    }
                    //Generic struct handling
                    if (propertyInfo.PropertyType.IsGenericType)
                    {
                        if (!propertyInfo.PropertyType.GetGenericTypeDefinition().IsValueType)
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

        private static void SerializeVariable(object variableType, byte[] array, int index, out int length)
        {
            //Set length to 0
            length = 0;
            //Perform serialization
            //TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Extension method for component, provides automatic serialization
        /// for commonly used types.
        /// </summary>
        /// <returns>Returns a byte array representing the component serialized.</returns>
        public static byte[] AutoSerialize(this Component component)
        {
            //Determine the length of what we are serializing
            throw new NotImplementedException();
        }

    }
}

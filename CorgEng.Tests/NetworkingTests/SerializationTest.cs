using CorgEng.Core;
using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Components;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Networking.Attributes;
using CorgEng.Networking.Components;
using CorgEng.UtilityTypes.Vectors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Tests.NetworkingTests
{
    [TestClass]
    public class SerializationTest
    {

        [UsingDependency]
        private static ILogger Logger;

        [TestMethod]
        [Timeout(10000)]
        public void TestComponentSerialization()
        {
            //Give it a seed, so tests are repeatable
            Random random = new Random(0);
            //Get all component classes
            IEnumerable<Type> components = CorgEngMain.LoadedAssemblyModules
                .SelectMany(assembly => assembly.GetTypes()
                .Where(type => typeof(Component).IsAssignableFrom(type)));
            Logger?.WriteLine($"Located {components.Count()} components to test.", LogType.LOG);
            //Begin testing
            foreach (Type type in components)
            {
                try
                {
                    if (type.IsAbstract)
                    {
                        Logger?.WriteLine($"Skipped {type.Name} (Abstract component)", LogType.DEBUG);
                        continue;
                    }
                    Component instantiatedComponent = (Component)FormatterServices.GetUninitializedObject(type);
                    //Randomise the properties with getters and setters
                    //Only test public properties (Private indicates it isn't networked)
                    IEnumerable<(bool, PropertyInfo)> componentProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.GetProperty)
                        .Where(propertyInfo => propertyInfo.SetMethod != null && propertyInfo.GetMethod != null && propertyInfo.GetCustomAttribute<NetworkSerializedAttribute>() != null)
                        .Select(propertyInfo => (propertyInfo.GetCustomAttribute<NetworkSerializedAttribute>().prototypeInclude, propertyInfo));
                    Dictionary<PropertyInfo, object> appliedPrototypeValues = new Dictionary<PropertyInfo, object>();
                    Dictionary<PropertyInfo, object> appliedInstanceValues = new Dictionary<PropertyInfo, object>();
                    //Randomise some properties
                    bool skip = true;
                    foreach ((bool, PropertyInfo) componentProperty in componentProperties)
                    {
                        object appliedValue;
                        //Switch the type
                        if (componentProperty.Item2.PropertyType == typeof(int) || componentProperty.Item2.PropertyType == typeof(short) || componentProperty.Item2.PropertyType == typeof(long))
                        {
                            appliedValue = random.Next(0, 50000);
                        }
                        else if (componentProperty.Item2.PropertyType == typeof(float) || componentProperty.Item2.PropertyType == typeof(double))
                        {
                            appliedValue = (float)random.NextDouble();
                        }
                        else if (componentProperty.Item2.PropertyType == typeof(char))
                        {
                            appliedValue = (char)random.Next(0, 255);
                        }
                        else if (typeof(Enum).IsAssignableFrom(componentProperty.Item2.PropertyType))
                        {
                            Array enumValues = Enum.GetValues(componentProperty.Item2.PropertyType);
                            appliedValue = enumValues.GetValue(random.Next(enumValues.Length));
                        }
                        else if (componentProperty.Item2.PropertyType == typeof(string))
                        {
                            string randomString = "";
                            for (int i = 0; i < 100; i++)
                            {
                                randomString = $"{randomString}{Convert.ToChar(random.Next(0, 127))}";
                            }
                            //Convert to ASCII
                            appliedValue = randomString;
                        }
                        else if (componentProperty.Item2.PropertyType == typeof(Vector<int>))
                        {
                            appliedValue = new Vector<int>(random.Next(0, 50000), random.Next(0, 50000), random.Next(0, 50000));
                        }
                        else if (componentProperty.Item2.PropertyType == typeof(Vector<float>))
                        {
                            appliedValue = new Vector<float>((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
                        }
                        else
                        {
                            Logger?.WriteLine($"Unrecognised type: {componentProperty.Item2.PropertyType}", LogType.WARNING);
                            continue;
                        }
                        skip = false;
                        componentProperty.Item2.SetValue(instantiatedComponent, appliedValue);
                        if (componentProperty.Item1)
                            appliedPrototypeValues.Add(componentProperty.Item2, appliedValue);
                        else
                            appliedInstanceValues.Add(componentProperty.Item2, appliedValue);
                    }
                    if (skip)
                    {
                        Logger?.WriteLine($"Skipped {type.Name} due to not having any properties we can work with!", LogType.WARNING);
                        continue;
                    }
                    //Serialize the data
                    //TEST FOR PROTOTYPES
                    byte[] serializedData = instantiatedComponent.AutoSerialize(true);
                    Logger?.WriteLine($"SERIALIZED COMPONENT (PROTOTYPE) {instantiatedComponent} (Length: {serializedData.Length}): {string.Join(",", serializedData)}", LogType.DEBUG);
                    //Deserialize
                    Component deserializedType = (Component)FormatterServices.GetUninitializedObject(type);
                    deserializedType.AutoDeserialize(serializedData, true);
                    //Verify
                    foreach (PropertyInfo property in appliedPrototypeValues.Keys)
                    {
                        Assert.AreEqual(property.GetValue(instantiatedComponent), property.GetValue(deserializedType), $"Deconversion failed on component of type {type} and variable {property.Name}");
                    }
                    //TEST FOR NON PROTOTYPES
                    serializedData = instantiatedComponent.AutoSerialize(false);
                    Logger?.WriteLine($"SERIALIZED COMPONENT {instantiatedComponent} (Length: {serializedData.Length}): {string.Join(",", serializedData)}", LogType.DEBUG);
                    //Deserialize
                    deserializedType = (Component)FormatterServices.GetUninitializedObject(type);
                    deserializedType.AutoDeserialize(serializedData, false);
                    //Verify
                    foreach (PropertyInfo property in appliedInstanceValues.Keys)
                    {
                        Assert.AreEqual(property.GetValue(instantiatedComponent), property.GetValue(deserializedType), $"Deconversion failed on component of type {type} and variable {property.Name}");
                    }
                    Logger?.WriteLine($"{type.Name} passed", LogType.LOG);
                }
                catch (Exception e) when (!(e is AssertFailedException) && !(e is AssertInconclusiveException))
                {
                    Assert.Fail($"Failed on {type.Name}\n{e}");
                }
            }
        }

        [TestMethod]
        [Timeout(10000)]
        public void TestEventSerialization()
        {
            //Give it a seed, so tests are repeatable
            Random random = new Random(0);
            //Get all event classes
            IEnumerable<Type> Events = CorgEngMain.LoadedAssemblyModules
                .SelectMany(assembly => assembly.GetTypes()
                .Where(type => typeof(INetworkedEvent).IsAssignableFrom(type)));
            Logger?.WriteLine($"Located {Events.Count()} events to test.", LogType.LOG);
            //Begin testing
            foreach (Type type in Events)
            {
                try
                {
                    if (type.IsAbstract)
                    {
                        Logger?.WriteLine($"Skipped {type.Name} (Abstract event)", LogType.LOG);
                        continue;
                    }
                    INetworkedEvent instantiatedEvent = (INetworkedEvent)FormatterServices.GetUninitializedObject(type);
                    //Randomise the properties with getters and setters
                    //Only test public properties (Private indicates it isn't networked)
                    IEnumerable<PropertyInfo> eventProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.GetProperty)
                        .Where(propertyInfo => propertyInfo.SetMethod != null && propertyInfo.GetMethod != null);
                    Dictionary<PropertyInfo, object> appliedValues = new Dictionary<PropertyInfo, object>();
                    //Randomise some properties
                    bool skip = true;
                    foreach (PropertyInfo eventProperty in eventProperties)
                    {
                        object appliedValue;
                        //Switch the type
                        if (eventProperty.PropertyType == typeof(int) || eventProperty.PropertyType == typeof(short) || eventProperty.PropertyType == typeof(long))
                        {
                            appliedValue = random.Next(0, 50000);
                        }
                        else if (eventProperty.PropertyType == typeof(float) || eventProperty.PropertyType == typeof(double))
                        {
                            appliedValue = (float)random.NextDouble();
                        }
                        else if (eventProperty.PropertyType == typeof(char))
                        {
                            appliedValue = (char)random.Next(0, 255);
                        }
                        else if (typeof(Enum).IsAssignableFrom(eventProperty.PropertyType))
                        {
                            Array enumValues = Enum.GetValues(eventProperty.PropertyType);
                            appliedValue = enumValues.GetValue(random.Next(enumValues.Length));
                        }
                        else if (eventProperty.PropertyType == typeof(string))
                        {
                            string randomString = "";
                            for (int i = 0; i < 100; i++)
                            {
                                randomString = $"{randomString}{(char)random.Next(0, 127)}";
                            }
                            appliedValue = randomString;
                        }
                        else if (eventProperty.PropertyType == typeof(Vector<int>))
                        {
                            appliedValue = new Vector<int>(random.Next(0, 50000), random.Next(0, 50000), random.Next(0, 50000));
                        }
                        else if (eventProperty.PropertyType == typeof(Vector<float>))
                        {
                            appliedValue = new Vector<float>((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
                        }
                        else
                        {
                            Logger?.WriteLine($"Unrecognised type: {eventProperty.PropertyType}", LogType.WARNING);
                            continue;
                        }
                        skip = false;
                        eventProperty.SetValue(instantiatedEvent, appliedValue);
                        appliedValues.Add(eventProperty, appliedValue);
                    }
                    if (skip)
                    {
                        Logger?.WriteLine($"Skipped {type.Name} due to not having any properties we can work with!", LogType.WARNING);
                        continue;
                    }
                    //Serialize the data
                    byte[] serializedData = new byte[instantiatedEvent.SerialisedLength()];
                    //Deserialize
                    INetworkedEvent deserializedType = (INetworkedEvent)FormatterServices.GetUninitializedObject(type);
                    using (MemoryStream memoryStream = new MemoryStream(serializedData))
                    {
                        //Writer
                        using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
                        {
                            Logger?.WriteLine($"SERIALIZED COMPONENT (Length: {serializedData.Length}): {string.Join(",", serializedData)}", LogType.DEBUG);
                            instantiatedEvent.Serialise(binaryWriter);
                        }
                    }
                    using (MemoryStream memoryStream = new MemoryStream(serializedData))
                    {
                        //Reader
                        using (BinaryReader binaryReader = new BinaryReader(memoryStream))
                        {
                            deserializedType.Deserialise(binaryReader);
                        }
                    }
                    //Verify
                    foreach (PropertyInfo property in appliedValues.Keys)
                    {
                        Assert.AreEqual(property.GetValue(instantiatedEvent), property.GetValue(deserializedType), $"Deconversion failed on event of type {type} and variable {property.Name}");
                    }
                    Logger?.WriteLine($"{type.Name} passed", LogType.LOG);
                }
                catch (Exception e) when (!(e is AssertFailedException) && !(e is AssertInconclusiveException))
                {
                    Assert.Fail($"Failed on {type.Name}\n{e}");
                }
            }
        }

    }
}

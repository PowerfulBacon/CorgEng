using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Components;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Networking.Attributes;
using CorgEng.Networking.Components;
using CorgEng.UtilityTypes.Vectors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
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

        private static bool setup = false;

        [TestInitialize]
        public void TestInit()
        {
            if (setup)
                return;
            setup = true;
            ComponentExtensions.LoadPropertyInfoCache();
        }

        [TestMethod]
        public void TestComponentSerialization()
        {
            //Give it a seed, so tests are repeatable
            Random random = new Random(0);
            //Get all component classes
            IEnumerable<Type> components = AppDomain.CurrentDomain.GetAssemblies()
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
                        Logger?.WriteLine($"Skipped {type.Name} (Abstract component)");
                        continue;
                    }
                    Component instantiatedComponent = (Component)FormatterServices.GetUninitializedObject(type);
                    //Randomise the properties with getters and setters
                    //Only test public properties (Private indicates it isn't networked)
                    IEnumerable<PropertyInfo> componentProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.GetProperty)
                        .Where(propertyInfo => propertyInfo.SetMethod != null && propertyInfo.GetMethod != null && propertyInfo.GetCustomAttribute<NetworkSerializedAttribute>() != null);
                    Dictionary<PropertyInfo, object> appliedValues = new Dictionary<PropertyInfo, object>();
                    //Randomise some properties
                    bool skip = true;
                    foreach (PropertyInfo componentProperty in componentProperties)
                    {
                        object appliedValue;
                        //Switch the type
                        if (componentProperty.PropertyType == typeof(int) || componentProperty.PropertyType == typeof(short) || componentProperty.PropertyType == typeof(long))
                        {
                            appliedValue = random.Next(0, 50000);
                        }
                        else if (componentProperty.PropertyType == typeof(float) || componentProperty.PropertyType == typeof(double))
                        {
                            appliedValue = (float)random.NextDouble();
                        }
                        else if (componentProperty.PropertyType == typeof(char))
                        {
                            appliedValue = (char)random.Next(0, 255);
                        }
                        else if (typeof(Enum).IsAssignableFrom(componentProperty.PropertyType))
                        {
                            Array enumValues = Enum.GetValues(componentProperty.PropertyType);
                            appliedValue = enumValues.GetValue(random.Next(enumValues.Length));
                        }
                        else if (componentProperty.PropertyType == typeof(string))
                        {
                            string randomString = "";
                            for (int i = 0; i < 100; i++)
                            {
                                randomString = $"{randomString}{Convert.ToChar(random.Next(0, 127))}";
                            }
                            //Convert to ASCII
                            appliedValue = randomString;
                        }
                        else if (componentProperty.PropertyType == typeof(Vector<int>))
                        {
                            appliedValue = new Vector<int>(random.Next(0, 50000), random.Next(0, 50000), random.Next(0, 50000));
                        }
                        else if (componentProperty.PropertyType == typeof(Vector<float>))
                        {
                            appliedValue = new Vector<float>((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
                        }
                        else
                        {
                            Logger?.WriteLine($"Unrecognised type: {componentProperty.PropertyType}", LogType.WARNING);
                            continue;
                        }
                        skip = false;
                        componentProperty.SetValue(instantiatedComponent, appliedValue);
                        appliedValues.Add(componentProperty, appliedValue);
                    }
                    if (skip)
                    {
                        Logger?.WriteLine($"Skipped {type.Name} due to not having any properties we can work with!", LogType.WARNING);
                        continue;
                    }
                    //Serialize the data
                    byte[] serializedData = instantiatedComponent.AutoSerialize();
                    Logger?.WriteLine($"SERIALIZED COMPONENT (Length: {serializedData.Length}): {string.Join(",", serializedData)}", LogType.DEBUG);
                    //Deserialize
                    Component deserializedType = (Component)FormatterServices.GetUninitializedObject(type);
                    deserializedType.AutoDeserialize(serializedData);
                    //Verify
                    foreach (PropertyInfo property in appliedValues.Keys)
                    {
                        Assert.AreEqual(property.GetValue(instantiatedComponent), property.GetValue(deserializedType), $"Deconversion failed on component of type {type} and variable {property.Name}");
                    }
                    Logger?.WriteLine($"{type.Name} passed");
                }
                catch (Exception e) when (!(e is AssertFailedException) && !(e is AssertInconclusiveException))
                {
                    Assert.Fail($"Failed on {type.Name}\n{e}");
                }
            }
        }

        [TestMethod]
        public void TestEventSerialization()
        {
            //Give it a seed, so tests are repeatable
            Random random = new Random(0);
            //Get all event classes
            IEnumerable<Type> Events = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes()
                .Where(type => typeof(Event).IsAssignableFrom(type)));
            Logger?.WriteLine($"Located {Events.Count()} events to test.", LogType.LOG);
            //Begin testing
            foreach (Type type in Events)
            {
                try
                {
                    if (type.IsAbstract)
                    {
                        Logger?.WriteLine($"Skipped {type.Name} (Abstract event)");
                        continue;
                    }
                    Event instantiatedEvent = (Event)FormatterServices.GetUninitializedObject(type);
                    if (!instantiatedEvent.IsSynced)
                    {
                        Logger?.WriteLine($"Skipped {type.Name} (Not a networked event)");
                        continue;
                    }
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
                    byte[] serializedData = instantiatedEvent.Serialize();
                    Logger?.WriteLine($"SERIALIZED COMPONENT (Length: {serializedData.Length}): {string.Join(",", serializedData)}", LogType.DEBUG);
                    //Deserialize
                    Event deserializedType = (Event)FormatterServices.GetUninitializedObject(type);
                    deserializedType.Deserialize(serializedData);
                    //Verify
                    foreach (PropertyInfo property in appliedValues.Keys)
                    {
                        Assert.AreEqual(property.GetValue(instantiatedEvent), property.GetValue(deserializedType), $"Deconversion failed on event of type {type} and variable {property.Name}");
                    }
                    Logger?.WriteLine($"{type.Name} passed");
                }
                catch (Exception e) when (!(e is AssertFailedException) && !(e is AssertInconclusiveException))
                {
                    Assert.Fail($"Failed on {type.Name}\n{e}");
                }
            }
        }

    }
}

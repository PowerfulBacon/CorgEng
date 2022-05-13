using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.GenericInterfaces.Logging;
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

        [TestMethod]
        public void TestSerialization()
        {
            Random random = new Random();
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
                    if (!instantiatedEvent.NetworkedEvent)
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
                                randomString = $"{randomString}{(char)random.Next(0, 255)}";
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

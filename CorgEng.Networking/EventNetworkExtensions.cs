﻿using CorgEng.Core.Dependencies;
using CorgEng.Core.Modules;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.GenericInterfaces.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Networking
{
    /// <summary>
    /// Provides extensions for the event class that add in networked IDs
    /// </summary>
    public static class EventNetworkExtensions
    {

        [UsingDependency]
        private static ILogger Logger;

        /// <summary>
        /// A dictionary containing the networked Event IDs by type.
        /// </summary>
        private static Dictionary<Type, ushort> networkedEventIDs = new Dictionary<Type, ushort>();

        public static int NetworkedID { get; private set; }

        /// <summary>
        /// Generates the networked IDs for all networked events.
        /// Deterministic, so they will always be in the same order
        /// (Client and server need to be synced)
        /// </summary>
        [ModuleLoad]
        public static void CreateNetworkedIDs()
        {
            //Use reflection to collect all event types
            //Sort the events alphabetically (They need to be sorted to be deterministic)
            Event e;
            IOrderedEnumerable<Type> LocatedEvents = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes()
                .Where(t => typeof(Event).IsAssignableFrom(t) && !t.IsAbstract &&
                    //Create a temporary instance to check if its networked
                    (e = (Event)FormatterServices.GetUninitializedObject(t)).NetworkedEvent))
                .OrderBy(networkedEvent =>
                    {
                        //return 0;
                        return networkedEvent.AssemblyQualifiedName;
                    });
            //Assign all events a non-0 ID.
            ushort number = 1;
            foreach (Type type in LocatedEvents)
            {
                networkedEventIDs.Add(type, number++);
            }
            //TODO:
            //Use hashing to generate a version ID
            int versionID = GenerateServerVersion();
            NetworkedID = versionID;
            //Print message
            Logger?.WriteLine($"Generated IDs for {number-1} networked events, current networked version ID: {versionID}", LogType.MESSAGE);
        }

        /// <summary>
        /// Returns the type of an event based on a provided network ID
        /// </summary>
        internal static Type GetTypeFromNetworkedID(ushort networkedID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Extension method that provides the networked ID for a given event.
        /// </summary>
        public static ushort GetNetworkedID(this Event targetEvent)
        {
            ushort output = 0;
            networkedEventIDs.TryGetValue(targetEvent.GetType(), out output);
            return output;
        }

        private static int GenerateServerVersion()
        {
            return -1430992642 + EqualityComparer<Dictionary<Type, ushort>>.Default.GetHashCode(networkedEventIDs);
        }
    }
}

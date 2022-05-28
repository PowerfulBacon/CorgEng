using CorgEng.Core.Dependencies;
using CorgEng.Core.Modules;
using CorgEng.EntityComponentSystem.Components;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.GenericInterfaces.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Networking.VersionSync
{
    internal class VersionGenerator
    {

        [UsingDependency]
        private static ILogger Logger;

        /// <summary>
        /// A dictionary containing the networked Event IDs by type.
        /// </summary>
        private static Dictionary<Type, ushort> networkedEventIDs = new Dictionary<Type, ushort>();

        private static Dictionary<ushort, Type> networkEventToType = new Dictionary<ushort, Type>();

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
                .Where(t => !t.IsAbstract &&
                    ((
                        typeof(Event).IsAssignableFrom(t) && 
                        //Create a temporary instance to check if its networked
                        (e = (Event)FormatterServices.GetUninitializedObject(t)).NetworkedEvent
                    ) || (
                        typeof(Component).IsAssignableFrom(t)
                    ))))
                .OrderBy(networkedEvent =>
                {
                    //return 0;
                    return networkedEvent.AssemblyQualifiedName;
                });
            //Assign all events a non-0 ID.
            ushort number = 1;
            foreach (Type type in LocatedEvents)
            {
                Logger.WriteLine($"NETWORK VERSION: #{number}: {type.AssemblyQualifiedName}", LogType.DEBUG);
                networkEventToType.Add(number, type);
                networkedEventIDs.Add(type, number++);
            }
            //TODO:
            //Use hashing to generate a version ID
            int versionID = GenerateServerVersion();
            NetworkedID = versionID;
            //Print message
            Logger?.WriteLine($"Generated IDs for {number - 1} networked events, current networked version ID: {versionID}", LogType.MESSAGE);
        }

        /// <summary>
        /// Returns the type of an event based on a provided network ID
        /// </summary>
        internal static Event GetEventFromNetworkedID(ushort networkedID)
        {
            return FormatterServices.GetUninitializedObject(networkEventToType[networkedID]) as Event;
        }

        /// <summary>
        /// Extension method that provides the networked ID for a given event.
        /// </summary>
        public static ushort GetNetworkedID(this Event targetEvent)
        {
            ushort output;
            networkedEventIDs.TryGetValue(targetEvent.GetType(), out output);
            return output;
        }

        /// <summary>
        /// Completely abritrary function, no idea how good or bad this is.
        /// </summary>
        private static int GenerateServerVersion()
        {
            int value = -1430992642;
            foreach (Type t in networkedEventIDs.Keys)
            {
                value = unchecked(17 * value + t.AssemblyQualifiedName.GetHashCode());
            }
            return value;
        }

    }
}

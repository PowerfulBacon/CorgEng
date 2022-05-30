﻿using CorgEng.Core.Dependencies;
using CorgEng.Core.Modules;
using CorgEng.EntityComponentSystem.Components;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Networking.VersionSync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Networking.VersionSync
{
    public static class VersionGenerator
    {

        [UsingDependency]
        private static ILogger Logger;

        /// <summary>
        /// A dictionary containing the networked Event IDs by type.
        /// </summary>
        private static Dictionary<Type, ushort> networkedTypeIds = new Dictionary<Type, ushort>();

        private static Dictionary<ushort, Type> networkIdToType = new Dictionary<ushort, Type>();

        public static int NetworkVersion { get; private set; }

        /// <summary>
        /// Generates the networked IDs for all networked events.
        /// Deterministic, so they will always be in the same order
        /// (Client and server need to be synced)
        /// </summary>
        [ModuleLoad]
        public static void CreateNetworkedIDs()
        {
            //Use reflection to collect all event types
            //Sort the types alphabetically (They need to be sorted to be deterministic)
            IVersionSynced e;
            IOrderedEnumerable<Type> LocatedTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes()
                .Where(t => !t.IsAbstract &&
                        typeof(IVersionSynced).IsAssignableFrom(t) && 
                        //Create a temporary instance to check if its networked
                        (e = (IVersionSynced)FormatterServices.GetUninitializedObject(t)).IsSynced
                    ))
                .OrderBy(networkedType =>
                {
                    //return 0;
                    return networkedType.AssemblyQualifiedName;
                });
            //Assign all events a non-0 ID.
            ushort number = 1;
            foreach (Type type in LocatedTypes)
            {
                Logger.WriteLine($"NETWORK VERSION: #{number}: {type.AssemblyQualifiedName}", LogType.DEBUG);
                networkIdToType.Add(number, type);
                networkedTypeIds.Add(type, number++);
            }
            //TODO:
            //Use hashing to generate a version ID
            int versionID = GenerateServerVersion();
            NetworkVersion = versionID;
            //Print message
            Logger?.WriteLine($"Generated IDs for {number - 1} networked types, current networked version ID: {versionID}", LogType.MESSAGE);
        }

        public static ushort GetNetworkedIdentifier(this IVersionSynced versionSyncedType)
        {
            ushort output;
            networkedTypeIds.TryGetValue(versionSyncedType.GetType(), out output);
            return output;
        }

        /// <summary>
        /// Returns an uninitialized object of the type represented by the identifier of that type.
        /// </summary>
        internal static T CreateTypeFromIdentifier<T>(ushort networkedID)
            where T : class
        {
            return FormatterServices.GetUninitializedObject(networkIdToType[networkedID]) as T;
        }

        /// <summary>
        /// Generates a version ID based on the names of the things being hashed.
        /// Completely abritrary hashing function, no idea how good or bad this is.
        /// </summary>
        private static int GenerateServerVersion()
        {
            int value = -1430992642;
            foreach (Type t in networkedTypeIds.Keys)
            {
                value = unchecked(17 * value + t.AssemblyQualifiedName.GetHashCode());
            }
            return value;
        }

    }
}
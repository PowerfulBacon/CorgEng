using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Networking.Config;
using CorgEng.GenericInterfaces.Networking.Networking;
using CorgEng.GenericInterfaces.Networking.Packets;
using CorgEng.GenericInterfaces.Networking.VersionSync;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using static CorgEng.EntityComponentSystem.Entities.Entity;
using static CorgEng.EntityComponentSystem.Systems.EntitySystem;

namespace CorgEng.EntityComponentSystem.Events
{

    public static class EventExtensions
    {

        [UsingDependency]
        private static INetworkConfig NetworkConfig;

        [UsingDependency]
        private static ILogger Logger;

        /// <summary>
        /// Raise this event against a specified target
        /// </summary>
        public static void Raise(
            this IEvent signal,
            IEntity target,
            bool synchronous = false,
            [CallerFilePath] string callingFile = "",
            [CallerMemberName] string callingMember = "",
            [CallerLineNumber] int callingLine = 0)
        {
            Logger.WriteLine($"Event raised {signal}", LogType.DEBUG_EVERYTHING);
            //Handle the signal
            target.HandleSignal(signal, synchronous, callingFile, callingMember, callingLine);
        }

        /// <summary>
        /// Raise this event and network it
        /// </summary>
        public static void Raise(
            this INetworkedEvent signal,
            IEntity target,
            bool synchronous = false,
            [CallerFilePath] string callingFile = "",
            [CallerMemberName] string callingMember = "",
            [CallerLineNumber] int callingLine = 0)
        {
            Logger.WriteLine($"Networked event raised {signal}", LogType.DEBUG_EVERYTHING);
            //Handle the signal
            target.HandleSignal(signal, synchronous, callingFile, callingMember, callingLine);
            //Inform the entity that networked event was raised
            //Skip directly to signal handling
            target.HandleSignal(new NetworkedEventRaisedEvent(signal), false, callingFile, callingMember, callingLine);
        }

        /// <summary>
        /// Raise the event globally.
        /// Use of synchronous is recommended only when there is no other options
        /// as it will freeze the running thread
        /// </summary>
        public static void RaiseGlobally(
            this IEvent signal,
            bool synchronous = false,
            [CallerFilePath] string callingFile = "",
            [CallerMemberName] string callingMember = "",
            [CallerLineNumber] int callingLine = 0)
        {
            Logger.WriteLine($"Event raised {signal}", LogType.DEBUG_EVERYTHING);
            //Check if we have any registered signals
            if (!EventManager.RegisteredEvents.ContainsKey(typeof(GlobalEventComponent)))
                return;
            //Locate all event types we are listening for
            EventComponentPair key = new EventComponentPair(signal.GetType(), typeof(GlobalEventComponent));
            //Locate the monitoring system's callback handler
            if (RegisteredSystemSignalHandlers.ContainsKey(key))
            {
                List<SystemEventHandlerDelegate> systemEventHandlers = RegisteredSystemSignalHandlers[key];
                for (int i = systemEventHandlers.Count - 1; i >= 0; i--)
                    systemEventHandlers[i].Invoke(null, null, signal, synchronous, callingFile, callingMember, callingLine);
            }
        }

        /// <summary>
        /// Raise the event globally
        /// </summary>
        public static void RaiseGlobally(
            this INetworkedEvent signal,
            bool sourcedLocally = true,
            bool synchronous = false,
            [CallerFilePath] string callingFile = "",
            [CallerMemberName] string callingMember = "",
            [CallerLineNumber] int callingLine = 0)
        {
            Logger.WriteLine($"Network event raised {signal}", LogType.DEBUG_EVERYTHING);
            //Check if we have any registered signals
            if (!EventManager.RegisteredEvents.ContainsKey(typeof(GlobalEventComponent)))
                return;
            //Locate all event types we are listening for
            EventComponentPair key = new EventComponentPair(signal.GetType(), typeof(GlobalEventComponent));
            //Locate the monitoring system's callback handler
            if (RegisteredSystemSignalHandlers.ContainsKey(key))
            {
                List<SystemEventHandlerDelegate> systemEventHandlers = RegisteredSystemSignalHandlers[key];
                for (int i = systemEventHandlers.Count - 1; i >= 0; i--)
                    systemEventHandlers[i].Invoke(null, null, signal, synchronous, callingFile, callingMember, callingLine);
            }
            //Don't relay messages coming from other clients already
            if (!sourcedLocally)
                return;
            //Don't relay client message
            if (!signal.CanBeRaisedByClient && (NetworkConfig?.ProcessClientSystems ?? true))
                return;
            //Inform globally that a networked event was raised
            //Skip directly to signal handling
            //Locate all event types we are listening for
            EventComponentPair networkKey = new EventComponentPair(typeof(NetworkedEventRaisedEvent), typeof(GlobalEventComponent));
            //Locate the monitoring system's callback handler
            if (!RegisteredSystemSignalHandlers.ContainsKey(networkKey))
            {
                return;
            }
            List<SystemEventHandlerDelegate> networkedEventHandlers = RegisteredSystemSignalHandlers[networkKey];
            for (int i = networkedEventHandlers.Count - 1; i >= 0; i--)
                networkedEventHandlers[i].Invoke(null, null, new NetworkedEventRaisedEvent(signal), synchronous, callingFile, callingMember, callingLine);
        }

    }
}

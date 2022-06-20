﻿using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Networking.Networking;
using CorgEng.GenericInterfaces.Networking.Packets;
using CorgEng.GenericInterfaces.Networking.VersionSync;
using System;
using System.Collections.Generic;
using static CorgEng.EntityComponentSystem.Entities.Entity;
using static CorgEng.EntityComponentSystem.Systems.EntitySystem;

namespace CorgEng.EntityComponentSystem.Events
{

    public static class EventExtensions
    {

        /// <summary>
        /// Raise this event against a specified target
        /// </summary>
        public static void Raise(this IEvent signal, IEntity target)
        {
            //Handle the signal
            target.HandleSignal(signal);
        }

        /// <summary>
        /// Raise this event and network it
        /// </summary>
        public static void Raise(this INetworkedEvent signal, IEntity target)
        {
            //Handle the signal
            target.HandleSignal(signal);
            //Inform the entity that networked event was raised
            //Skip directly to signal handling
            target.HandleSignal(new NetworkedEventRaisedEvent(signal));
        }

        /// <summary>
        /// Raise the event globally
        /// </summary>
        public static void RaiseGlobally(this IEvent signal)
        {
            //Check if we have any registered signals
            if (!EventManager.RegisteredEvents.ContainsKey(typeof(GlobalEventComponent)))
                return;
            //Locate all event types we are listening for
            EventComponentPair key = new EventComponentPair(signal.GetType(), typeof(GlobalEventComponent));
            //Locate the monitoring system's callback handler
            if (RegisteredSystemSignalHandlers.ContainsKey(key))
            {
                List<SystemEventHandlerDelegate> systemEventHandlers = RegisteredSystemSignalHandlers[key];
                foreach (SystemEventHandlerDelegate systemEventHandler in systemEventHandlers)
                    systemEventHandler.Invoke(null, null, signal);
            }
        }

        /// <summary>
        /// Raise the event globally
        /// </summary>
        public static void RaiseGlobally(this INetworkedEvent signal, bool sourcedLocally = true)
        {
            //Check if we have any registered signals
            if (!EventManager.RegisteredEvents.ContainsKey(typeof(GlobalEventComponent)))
                return;
            //Locate all event types we are listening for
            EventComponentPair key = new EventComponentPair(signal.GetType(), typeof(GlobalEventComponent));
            //Locate the monitoring system's callback handler
            if (RegisteredSystemSignalHandlers.ContainsKey(key))
            {
                List<SystemEventHandlerDelegate> systemEventHandlers = RegisteredSystemSignalHandlers[key];
                foreach (SystemEventHandlerDelegate systemEventHandler in systemEventHandlers)
                    systemEventHandler.Invoke(null, null, signal);
            }
            //Don't relay messages coming from other clients already
            if (!sourcedLocally)
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
            foreach (SystemEventHandlerDelegate systemEventHandler in networkedEventHandlers)
                systemEventHandler.Invoke(null, null, new NetworkedEventRaisedEvent(signal));
        }

    }
}

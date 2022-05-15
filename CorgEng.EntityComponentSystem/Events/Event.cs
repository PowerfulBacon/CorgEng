﻿using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.GenericInterfaces.Networking.Networking;
using CorgEng.GenericInterfaces.Networking.Packets;
using System;
using System.Collections.Generic;
using static CorgEng.EntityComponentSystem.Entities.Entity;
using static CorgEng.EntityComponentSystem.Systems.EntitySystem;

namespace CorgEng.EntityComponentSystem.Events
{
    public abstract class Event
    {

        /// <summary>
        /// If true, this event will be networked.
        /// </summary>
        public abstract bool NetworkedEvent { get; }

        /// <summary>
        /// Raise this event against a specified target
        /// </summary>
        public void Raise(Entity target)
        {
            //Handle the signal
            target.HandleSignal(this);
            //Inform the entity that networked event was raised
            if (NetworkedEvent)
            {
                //Skip directly to signal handling
                target.HandleSignal(new NetworkedEventRaisedEvent(this));
            }
        }

        /// <summary>
        /// Raise the event globally
        /// </summary>
        public void RaiseGlobally()
        {
            //Check if we have any registered signals
            if (!EventManager.RegisteredEvents.ContainsKey(typeof(GlobalEventComponent)))
                return;
            //Locate all event types we are listening for
            EventComponentPair key = new EventComponentPair(GetType(), typeof(GlobalEventComponent));
            //Locate the monitoring system's callback handler
            if (!RegisteredSystemSignalHandlers.ContainsKey(key))
            {
                return;
            }
            List<SystemEventHandlerDelegate> systemEventHandlers = RegisteredSystemSignalHandlers[key];
            foreach (SystemEventHandlerDelegate systemEventHandler in systemEventHandlers)
                systemEventHandler.Invoke(null, null, this);
        }

        /// <summary>
        /// Serialize this event for network transmission
        /// </summary>
        public virtual byte[] Serialize() => throw new NotImplementedException();

        /// <summary>
        /// Deserialize a serialized event
        /// </summary>
        public virtual void Deserialize(byte[] data) => throw new NotImplementedException();

    }
}

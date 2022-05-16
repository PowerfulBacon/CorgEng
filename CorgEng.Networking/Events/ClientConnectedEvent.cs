﻿using CorgEng.EntityComponentSystem.Events;
using CorgEng.GenericInterfaces.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Networking.Events
{
    /// <summary>
    /// Called when a client is connected
    /// </summary>
    public class ClientConnectedEvent : Event
    {

        public override bool NetworkedEvent => false;

        public IClient Client { get; set; }

        public ClientConnectedEvent(IClient client)
        {
            Client = client;
        }

    }
}
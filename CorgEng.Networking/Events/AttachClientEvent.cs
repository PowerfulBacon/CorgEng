﻿using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Networking.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Networking.Events
{
    public class AttachClientEvent : IEvent
    {

        public IClient Client { get; set; }

        public AttachClientEvent(IClient client)
        {
            Client = client;
        }
    }
}

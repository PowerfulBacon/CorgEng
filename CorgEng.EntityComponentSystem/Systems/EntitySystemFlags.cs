﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Systems
{
    public enum EntitySystemFlags
    {
        //Does the entity system run while the server is running?
        HOST_SYSTEM = 1 << 0,
        //Does the entity system run while the client is running?
        CLIENT_SYSTEM = 1 << 1,
        //Force asynchronous events to be multi-threaded even if we can execute them immediately
        NO_IMMEDIATE_ASYNCHRONOUS_EVENTS = 1 << 2,
    }
}

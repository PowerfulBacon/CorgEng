﻿using CorgEng.EntityComponentSystem.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Events.Events
{
    public class ComponentRemovedEvent : Event
    {

        public Component Component { get; set; }

        public override bool NetworkedEvent { get; } = false;

        public ComponentRemovedEvent(Component component, bool networked)
        {
            Component = component;
            NetworkedEvent = networked;
        }
    }
}

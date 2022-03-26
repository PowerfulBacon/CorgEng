using CorgEng.EntityComponentSystem.Components;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events;
using System;

namespace CorgEng.EntityComponentSystem.Systems
{
    public abstract class System
    {

        internal delegate void SystemEventHandlerDelegate(Entity entity, Component component, Event signal);

        public abstract void SystemSetup();

        /// <summary>
        /// Register to a local event
        /// </summary>
        public void RegisterLocalEvent<GComponent, GEvent>(Func<Entity, GComponent, GEvent> eventHandler)
        {
            
        }

    }
}

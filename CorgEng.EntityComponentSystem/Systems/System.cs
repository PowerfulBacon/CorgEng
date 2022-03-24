using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Systems
{
    public abstract class System
    {

        public abstract void SystemSetup();

        /// <summary>
        /// Register to a local event
        /// </summary>
        public void RegisterLocalEvent<GComponent, GEvent>(Func<Entity, GComponent, GEvent> eventHandler)
        {
            if (EventManager.RegisteredComponentEvents.ContainsKey(typeof(GComponent)))
                EventManager.RegisteredComponentEvents[typeof(GComponent)].Add(typeof(GEvent));
            else
                EventManager.RegisteredComponentEvents.Add(typeof(GComponent), new List<Type>() { typeof(GEvent) });
        }

    }
}

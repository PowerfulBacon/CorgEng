using CorgEng.EntityComponentSystem.Components;
using CorgEng.EntityComponentSystem.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Entities
{
    public class Entity
    {

        List<Component> Components { get; } = new List<Component>();

        Dictionary<Type, Func<Entity, Component, Event>> EventListeners;

    }
}

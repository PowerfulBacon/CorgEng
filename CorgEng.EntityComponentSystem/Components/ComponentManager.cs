using CorgEng.EntityComponentSystem.Components.Exceptions;
using CorgEng.EntityComponentSystem.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Components
{
    public static class ComponentManager
    {

        /// <summary>
        /// Add a component to the specified entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="component"></param>
        public static void AddComponent(Entity entity, Component component)
        {
            if (component.Parent != null)
                throw new ComponentException($"Cannot add component {component} to {entity}, it already is attached to {component.Parent}");
            //Attach the component to the specified entity.
            entity.Components.Add(component);
            component.Parent = entity;
        }

    }
}

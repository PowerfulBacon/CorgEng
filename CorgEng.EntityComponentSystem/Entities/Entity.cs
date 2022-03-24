using CorgEng.EntityComponentSystem.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Entities
{
    public class Entity
    {

        /// <summary>
        /// A dictionary shared between all entities that records an entity and the component
        /// instances it has.
        /// Shared to save on memory.
        /// </summary>
        private static Dictionary<(Entity, Type), Component> EntityComponentInstances { get; } = new Dictionary<(Entity, Type), Component>();

        /// <summary>
        /// A list of the component types attatched to this entity
        /// </summary>
        private List<Type> ComponentTypes { get; } = new List<Type>();

        /// <summary>
        /// TODO: Populate this.
        /// A dictionary that associates event types to a list of callbacks that this entity
        /// needs to call should it recieve that event.
        /// </summary>
        public Dictionary<Type, Func<>>

        /// <summary>
        /// Get the component with the specified type
        /// </summary>
        public Component GetComponent<ComponentType>()
        {
            return EntityComponentInstances[(this, typeof(ComponentType))];
        }

        /// <summary>
        /// Delete the entity, removes it's references from the static list.
        /// </summary>
        public void Delete()
        {
            foreach (Type type in ComponentTypes)
            {
                EntityComponentInstances.Remove((this, type));
            }
        }

    }
}

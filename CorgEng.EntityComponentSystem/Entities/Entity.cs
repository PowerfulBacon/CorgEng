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
        /// List of components attached to this entity
        /// </summary>
        public List<Component> Components { get; } = new List<Component>();

    }
}

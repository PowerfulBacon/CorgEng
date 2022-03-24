using CorgEng.EntityComponentSystem.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Components
{
    public abstract class Component
    {
        
        /// <summary>
        /// The parent of this component
        /// </summary>
        public Entity Parent { get; internal set; }

    }
}

using CorgEng.EntityComponentSystem.Components;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Contents.Components
{
    public class ContainedComponent : Component
    {

        /// <summary>
        /// The entity that we are contained inside.
        /// </summary>
        public IEntity Holder { get; internal set; }

        public ContainedComponent(IEntity holder)
        {
            Holder = holder;
        }
    }
}

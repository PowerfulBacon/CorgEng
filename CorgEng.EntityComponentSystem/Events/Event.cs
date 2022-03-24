using CorgEng.EntityComponentSystem.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Events
{
    public abstract class Event
    {

        /// <summary>
        /// Raise this event against a specified target
        /// </summary>
        public void Raise(Entity target)
        {

        }

    }
}

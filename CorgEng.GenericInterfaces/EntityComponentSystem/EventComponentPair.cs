using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.EntityComponentSystem
{
    public struct EventComponentPair
    {

        public Type EventType { get; }

        public Type ComponentType { get; }

        public EventComponentPair(Type eventType, Type componentType)
        {
            EventType = eventType;
            ComponentType = componentType;
        }
    }
}

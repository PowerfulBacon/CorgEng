using System;
using System.Collections.Generic;

namespace CorgEng.EntityComponentSystem.Events
{
    internal static class EventManager
    {

        /// <summary>
        /// Matches component type to types of registered events
        /// </summary>
        internal static Dictionary<Type, List<Type>> RegisteredEvents = new Dictionary<Type, List<Type>>();
        
    }
}

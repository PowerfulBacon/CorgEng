using System;

namespace CorgEng.EntityComponentSystem.Components.Exceptions
{
    public class ComponentException : Exception
    {
        public ComponentException(string message) : base(message)
        { }
    }
}

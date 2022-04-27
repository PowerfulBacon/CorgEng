using System;

namespace CorgEng.DependencyInjection.Dependencies
{
    /// <summary>
    /// Defines a dependency, something that can be injected
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DependencyAttribute : Attribute
    {

        //This dependency will not be loaded by default
        public int priority = 0;

    }
}

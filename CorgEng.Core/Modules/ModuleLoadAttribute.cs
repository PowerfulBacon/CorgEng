using System;

namespace CorgEng.Core.Modules
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ModuleLoadAttribute : Attribute
    {

        public bool priority = false;

    }
}

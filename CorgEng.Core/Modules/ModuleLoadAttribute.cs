using JetBrains.Annotations;
using System;

namespace CorgEng.Core.Modules
{
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Method)]
    public class ModuleLoadAttribute : Attribute
    {

        public bool priority = false;

        public bool mainThread = false;

    }
}

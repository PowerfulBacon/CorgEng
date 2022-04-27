using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Core.Modules
{
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Method)]
    public class ModuleTerminateAttribute : Attribute
    {
    }
}

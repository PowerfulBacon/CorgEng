using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.DependencyInjection.Dependencies
{
    /// <summary>
    /// Indicdates that an interface should not be used
    /// as a dependency and will cause dependency injection
    /// to fail. This is used when an interface that was a dependency
    /// before is no longer being used as a dependency.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    internal class NonDependencyAttribute : Attribute
    {

        public bool fail = true;

        public string message;

    }
}

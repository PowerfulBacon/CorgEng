using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.DependencyInjection.Injection
{
    [Dependency]
    internal class DependencyFetcher : IDependencyFetcher
    {

        public object GetDependency(Type dependencyType)
        {
            return DependencyInjector.GetDependencyInstance(dependencyType);
        }

    }
}

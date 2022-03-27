using System.Collections.Generic;

namespace CorgEng.DependencyInjection.Injection
{
    public class DependencyList
    {

        public object DefaultDependency { get; set; }

        public List<object> LoadedDependencies { get; set; } = new List<object>();

    }
}

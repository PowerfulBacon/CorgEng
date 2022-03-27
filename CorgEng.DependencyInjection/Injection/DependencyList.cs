using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.DependencyInjection.Injection
{
    public class DependencyList
    {

        public object DefaultDependency { get; set; }

        public List<object> LoadedDependencies { get; set; } = new List<object>();

    }
}

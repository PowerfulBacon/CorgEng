using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Components.Exceptions
{
    public class ComponentException : Exception
    {
        public ComponentException(string message) : base(message)
        { }
    }
}

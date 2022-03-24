using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Core.Rendering.Exceptions
{
    public class NullRenderCoreException : Exception
    {
        public NullRenderCoreException(string message) : base(message)
        { }
    }
}

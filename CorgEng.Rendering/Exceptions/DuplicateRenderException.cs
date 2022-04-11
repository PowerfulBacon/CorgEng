using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Rendering.Exceptions
{
    public class DuplicateRenderException : Exception
    {
        public DuplicateRenderException(string message) : base(message)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Networking.Exceptions
{
    internal class NetworkingException : Exception
    {
        public NetworkingException(string message) : base(message)
        {
        }

        public NetworkingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

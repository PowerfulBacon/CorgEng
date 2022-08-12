using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.ContentLoading
{
    internal class ContentLoadException : Exception
    {
        public ContentLoadException()
        {
        }

        public ContentLoadException(string message) : base(message)
        {
        }

        public ContentLoadException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ContentLoadException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

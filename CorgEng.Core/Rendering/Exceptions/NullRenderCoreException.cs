using System;

namespace CorgEng.Core.Rendering.Exceptions
{
    public class NullRenderCoreException : Exception
    {
        public NullRenderCoreException(string message) : base(message)
        { }
    }
}

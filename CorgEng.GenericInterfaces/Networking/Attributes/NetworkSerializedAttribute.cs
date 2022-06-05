using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Networking.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NetworkSerializedAttribute : Attribute
    {

        public bool prototypeInclude = true;

    }
}

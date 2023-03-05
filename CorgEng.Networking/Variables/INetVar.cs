using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Networking.Variables
{
    internal interface INetVar
    {

        Type GetStoredType();

        object GetValue();

    }
}

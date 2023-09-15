using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.NetworkingPrimitives.Variables
{
    internal interface INetVar
    {

        Type GetStoredType();

        object GetValue();

    }
}

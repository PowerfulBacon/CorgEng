using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Networking.PrototypeManager
{
    public interface IPrototype
    {

        /// <summary>
        /// The unique identifier of this prototype.
        /// 4 byte integer value. Allows for 4 billion prototypes, which should be
        /// basically impossible to reach.
        /// </summary>
        int Identifier { get; }

    }
}

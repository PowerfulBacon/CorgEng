using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Networking.Packets
{
    public interface INetworkMessage
    {

        /// <summary>
        /// Get the bytes of this message
        /// </summary>
        byte[] GetBytes();

    }
}

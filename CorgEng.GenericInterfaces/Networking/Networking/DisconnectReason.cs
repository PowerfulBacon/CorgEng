using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Networking.Networking
{
    public enum DisconnectReason
    {
        //Timed out from the server
        TIMEOUT,
        //Connection was rejected by the serve
        CONNECTION_REJECTED
    }
}

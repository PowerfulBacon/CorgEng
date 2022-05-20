using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Networking.Packets
{

    public enum PacketHeaders
    {
        //Request to connect
        CONNECTION_REQUEST,
        //Connection accepted
        CONNECTION_ACCEPT,
        //Connection rejected
        CONNECTION_REJECT,
        //On event raised
        EVENT_RAISED,
        //Networking test message
        NETWORKING_TEST,
    }
}

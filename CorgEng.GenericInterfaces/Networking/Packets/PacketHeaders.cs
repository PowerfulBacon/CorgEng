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
        GLOBAL_EVENT_RAISED,
        //Local event raised
        LOCAL_EVENT_RAISED,
        //Networking test message
        NETWORKING_TEST,
        //Recieved information about a prototype
        PROTOTYPE_INFO,
        //Client requests information about a prototype
        REQUEST_PROTOTYPE,
        //Information about an entity
        ENTITY_DATA,
        //Tells a client to update their view and move it to a new location
        UPDATE_CLIENT_VIEW,
        //Request information about an entity
        REQUEST_ENTITY,
    }
}

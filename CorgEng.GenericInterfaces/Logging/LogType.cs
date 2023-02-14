/// <summary>
/// Level of the log type
/// </summary>
namespace CorgEng.GenericInterfaces.Logging
{
    public enum LogType
    {
        LOG_ALL = ~0,
        DEBUG_EVERYTHING = 1 << 0,  //Literally everything
        DEBUG = 1 << 1,      //Debug messages, will be ignored unless logger is set to log debug messages
        MESSAGE = 1 << 2,    //Standard messages such as player connections
        LOG = 1 << 3,        //Log messages about player interactions on the server
        WARNING = 1 << 4,    //Warning messages such as invalid configs or something
        ERROR = 1 << 5,      //Error messages for exceptions
        TEMP = 1 << 6,       //Temp message for debugging, super obvious and easy to remove
        NETWORK_LOG = 1 << 7,   //A networking log
    }
}

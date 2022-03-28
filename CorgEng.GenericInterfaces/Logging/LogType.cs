/// <summary>
/// Level of the log type
/// </summary>
namespace CorgEng.GenericInterfaces.Logging
{
    public enum LogType
    {
        LOG_ALL = ~0,
        DEBUG = 1 << 0,      //Debug messages, will be ignored unless logger is set to log debug messages
        MESSAGE = 1 << 1,    //Standard messages such as player connections
        LOG = 1 << 2,        //Log messages about player interactions on the server
        WARNING = 1 << 3,    //Warning messages such as invalid configs or something
        ERROR = 1 << 4,      //Error messages for exceptions
        TEMP = 1 << 5,       //Temp message for debugging, super obvious and easy to remove
    }
}

﻿namespace CorgEng.GenericInterfaces.Logging
{
    public interface ILogger
    {

        void WriteLine(object message, LogType logType = LogType.MESSAGE);

    }
}

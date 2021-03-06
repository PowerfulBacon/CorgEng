using CorgEng.Core.Modules;
using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CorgEng.Logging
{
    [Dependency]
    public class ConsoleLogger : ILogger
    {

        private static object consoleLock = new object();

        private static LogType LogFlags = LogType.LOG_ALL & ~LogType.DEBUG_EVERYTHING;
        //private static LogType LogFlags = LogType.LOG_ALL;

        public static int ExceptionCount = 0;

        //TODO: Make this a system
        public void WriteLine(object message, LogType logType = LogType.MESSAGE)
        {
            if (message is Exception)
                ExceptionCount++;
            //Ignore this log
            if ((logType & LogFlags) != logType)
                return;
            DateTime logTime = DateTime.Now;
            string logText = $"[{Thread.CurrentThread.Name ?? $"T{Thread.CurrentThread.ManagedThreadId}"}][{logType}][{logTime}]";
            lock (consoleLock)
            {
                //Write it
                SetConsoleColor(logType);
                Console.Write(logText);
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine($" {message ?? "null"}");
            }
        }

        private void SetConsoleColor(LogType logType)
        {
            switch (logType)
            {
                case LogType.DEBUG_EVERYTHING:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    return;
                case LogType.DEBUG:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    return;
                case LogType.ERROR:
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                case LogType.WARNING:
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    Console.ForegroundColor = ConsoleColor.Black;
                    return;
                case LogType.MESSAGE:
                    Console.ForegroundColor = ConsoleColor.Green;
                    return;
                case LogType.LOG:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    return;
                case LogType.TEMP:
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                    return;
            }
        }

    }
}

﻿using CorgEng.Core.Modules;
using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.Logging;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace CorgEng.Logging
{
    [Dependency]
    public class ConsoleLogger : ILogger
    {

        private static object consoleLock = new object();

        private static LogType ConsoleLogFlags = LogType.LOG_ALL & ~(LogType.DEBUG_EVERYTHING | LogType.NETWORK_LOG);

        private static LogType LogFlags = LogType.LOG_ALL & ~(LogType.DEBUG_EVERYTHING | LogType.DEBUG);
        //private static LogType LogFlags = LogType.LOG_ALL;

        public static int ExceptionCount = 0;

        private static volatile bool _isFileWriting = false;

        private static ConcurrentQueue<string> _logWritingQueue = new ConcurrentQueue<string>();
        private static ConcurrentQueue<string> _metricWritingQueue = new ConcurrentQueue<string>();

        private static object _logWritingLock = new object();

        public static string LogPath { get; set; } = $"Logs";

        public static string LogFile { get; set; } = $"LogOutput_{AppDomain.CurrentDomain.FriendlyName}.txt";
        public static string MetricFile { get; set; } = $"Metrics_{AppDomain.CurrentDomain.FriendlyName}.csv";

        private static StreamWriter _logFile;
        private static StreamWriter _csvFile;

        private static bool _fileWritingDisabled = false;

        //TODO: Make this a system
        public void WriteLine(object message, LogType logType)
        {
            if (message is Exception)
                ExceptionCount++;
            
            DateTime logTime = DateTime.Now;
            string logText = $"[{Thread.CurrentThread.Name ?? $"T{Thread.CurrentThread.ManagedThreadId}"}][{logType}][{logTime}]";
            lock (consoleLock)
            {
                //Ignore this log
                if ((logType & ConsoleLogFlags) == logType)
                {
                    //Write it
                    SetConsoleColor(logType);
                    Console.Write(logText);
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine($" {message ?? "null"}");
                }
            }
            if ((logType & LogFlags) == logType)
                return;
            if (_fileWritingDisabled)
                return;
            _logWritingQueue.Enqueue($"{logText} {message ?? null}");
            StartLogWritingQueue();
        }

        private void StartLogWritingQueue()
        {
            lock (_logWritingLock)
            {
                if (!_isFileWriting)
                {
                    _isFileWriting = true;
                    if (_logFile == null)
                    {
                        try
                        {
                            Directory.CreateDirectory(LogPath);
                            _logFile = new StreamWriter(File.Create($"{LogPath}/{LogFile}"));
                            _csvFile = new StreamWriter(File.Create($"{LogPath}/{MetricFile}"));
                            _csvFile.WriteLine("metric_name, timestamp, metric_value");
                        }
                        catch (Exception e)
                        {
                            WriteLine($"An exception occured initialising the log file, log to file has been disabled.\n{e}", LogType.ERROR);
                            _fileWritingDisabled = true;
                        }
                    }
                    //A lot of horrible code for handling async things

                    Task.Run(() =>
                    {
                        while (true)
                        {
                            lock (_logWritingLock)
                            {
                                bool passed = false;
                                if (_logWritingQueue.TryDequeue(out string line))
                                {
                                    _logFile.WriteLine(line);
                                    passed = true;
                                }
                                if (_metricWritingQueue.TryDequeue(out string metric))
                                {
                                    _csvFile.WriteLine(metric);
                                    passed = true;
                                }
                                if (!passed)
                                {
                                    _isFileWriting = false;
                                    break;
                                }
                            }
                        }
                    });
                }
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
                case LogType.NETWORK_LOG:
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    return;
                case LogType.TEMP:
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                    return;
            }
        }

        public void WriteMetric(string metricName, string metricValue)
        {
            if (_fileWritingDisabled)
                return;
            _metricWritingQueue.Enqueue($"{metricName}, {Core.CorgEngMain.Time}, {metricValue}");
            StartLogWritingQueue();
        }
    }
}

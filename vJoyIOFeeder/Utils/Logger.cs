using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace vJoyIOFeeder.Utils
{
    public enum LogLevels : int
    {
        ERROR = 0,
        IMPORTANT,
        INFORMATIVE,
        DEBUG
    }
    public struct LogMessage
    {
        public string Message;
        public double Timestamp;
        public LogLevels Level;

        public LogMessage(string message, LogLevels level = LogLevels.INFORMATIVE)
        {
            Timestamp = MultimediaTimer.RefTimer.Elapsed.TotalSeconds;
            Message = message;
            Level = level;
        }

        public LogMessage(LogMessage message)
        {
            Timestamp = message.Timestamp;
            Message = message.Message;
            Level = message.Level;
        }
    }

    public static class Logger
    {
        public static LogLevels LogLevel = LogLevels.INFORMATIVE;

        public delegate void LogMethod(string text);
        public static event LogMethod Loggers;


        static bool Running = true;
        static Thread LoggerThread = null;
        public static void Start()
        {
            if (LoggerThread != null) {
                Stop();
            }
            LoggerThread = new Thread(LoggerThreadMethod);
            Running = true;
            LoggerThread.Name = "Logger";
            LoggerThread.Priority = ThreadPriority.BelowNormal;
            LoggerThread.Start();
        }
        public static void Stop()
        {
            Running = false;
            if (LoggerThread == null)
                return;
            Thread.Sleep(100);
            LoggerThread.Join();
            LoggerThread = null;
        }


        static ConcurrentQueue<LogMessage> LogStack = new ConcurrentQueue<LogMessage>();


        static void LoggerThreadMethod()
        {
            while (Running) {
                if (LogStack.IsEmpty) {
                    Thread.Sleep(100);
                    continue;
                }
                int count = 0;
                while(PrintOne() && count++<50) {
                }
            }
        }

        public static bool PrintOne()
        {
            StringBuilder text = new StringBuilder();
            if (LogStack.IsEmpty)
                return false;
            if (!LogStack.TryDequeue(out var msg))
                return false;
            if (Loggers == null)
                return true;
            text.Append(msg.Level.ToString().Substring(0, 5));
            text.Append("|");
            text.Append(string.Format("{0,12:F6}", msg.Timestamp));
            text.Append(":");
            text.Append(msg.Message);
            Loggers(text.ToString());
            return true;
        }

        public static void Log(string message)
        {
            var msg = new LogMessage(message, LogLevels.INFORMATIVE);
            LogStack.Enqueue(msg);
        }

        public static void Log(string message, LogLevels level = LogLevels.DEBUG)
        {
            if (level > LogLevel)
                return;
            var msg = new LogMessage(message, level);
            LogStack.Enqueue(msg);
        }
        public static void LogFormat(LogLevels level, string text, params object[] args)
        {
            Log(String.Format(text, args), level);
        }
        public static void Log(LogMessage message)
        {
            if (message.Level > LogLevel)
                return;
            var msg = new LogMessage(message);
            LogStack.Enqueue(msg);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axe.Logging.Core
{
    public static class AxeLoggingExtensions
    {
        public const string LogEntryAddToExceptionKey = "Axe_Log_Content";

        public static T Mark<T>(this T exception, LogEntry logEntry) where T : Exception
        {
            exception.Data[LogEntryAddToExceptionKey] = logEntry;
            return exception;
        }

        public static List<LogEntry> GetLogEntries(this Exception exception, int maxLevel = 10)
        {
            var defaultLogEntry = new LogEntry
            {
                Entry = exception.Message,
                Data = new {Error = exception.ToString()},
                Level = LoggingLevel.Error
            };

            var allLogEntries = new List<LogEntry>();
            if (exception != null)
            {
                LogEntry currentLogEntry = exception.Data[LogEntryAddToExceptionKey] as LogEntry;
                if (currentLogEntry != null)
                {
                    allLogEntries.Add(currentLogEntry);
                }
                else
                {
                    allLogEntries.Add(defaultLogEntry);
                }
            }

            return allLogEntries;
        }
    }
}

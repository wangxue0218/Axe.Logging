using System;
using System.Collections.Generic;
using System.Linq;

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
            int currentLevel = maxLevel;
            bool IsAllExceptionsMarked = GetInnerExceptionLogEntries(allLogEntries, exception, currentLevel);

            if (!IsAllExceptionsMarked)
            {
                allLogEntries.Add(defaultLogEntry);
            }

            return allLogEntries;
        }

        static bool GetInnerExceptionLogEntries(List<LogEntry> allLogEntries, Exception exception, int allLevel)
        {
            if (allLevel < 0) return false;
            int currentLevel = allLevel;
            Exception currentException = exception;
            while (currentLevel >= 0 && currentException != null)
            {
                LogEntry currentLogEntry = currentException.Data[LogEntryAddToExceptionKey] as LogEntry;
                if (currentLogEntry != null)
                {
                    allLogEntries.Add(currentLogEntry);
                    return true;
                }
                currentLevel--;
                AggregateException aggregateException = currentException as AggregateException;
                currentException = currentException.InnerException;

                if (aggregateException == null)
                {
                    continue;
                }

                bool allInnerExceptionMarked = true;
                foreach (Exception ex in aggregateException.Flatten().InnerExceptions)
                {
                    bool aggregateExceptionMarked = GetInnerExceptionLogEntries(allLogEntries, ex, currentLevel);
                    if (!aggregateExceptionMarked) allInnerExceptionMarked = false;
                }

                return allInnerExceptionMarked;
            }

            return false;
        }
    }
}

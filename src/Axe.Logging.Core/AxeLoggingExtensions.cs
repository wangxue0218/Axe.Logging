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
    }
}

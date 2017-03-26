using System;
using System.Linq;
using Axe.Logging.Core;
using Xunit;

namespace Axe.Logging.Test
{
    public class when_get_exception_logging_entries
    {
        [Fact]
        public void should_return_logging_entry_of_error_level_when_simple_exception_is_not_marked()
        {
            var exception = new Exception();
            LogEntry logEntry = exception.GetLogEntries();
            Assert.NotNull(logEntry);
            Assert.Equal(LoggingLevel.Error, logEntry.Level);
        }
        [Theory]
        [InlineData(LoggingLevel.Info)]
        [InlineData(LoggingLevel.Warn)]
        [InlineData(LoggingLevel.Error)]
        [InlineData(LoggingLevel.Fatal)]
        public void should_return_logging_entry_when_simple_exception_is_marked(LoggingLevel level)
        {
            var logEntry = new LogEntry
            {
                Id = Guid.NewGuid(),
                Time = DateTime.UtcNow,
                Entry = "entry uri + httpMethod",
                User = new { CurrentUser = "Lou + pwc001" },
                Data = new { Parameters = "I do not care" },
                Level = level
            };

            var exception = new Exception();
            exception.Mark(logEntry);
            LogEntry getLogEntry = exception.GetLogEntries();
            Assert.NotNull(getLogEntry);
            Assert.Equal(getLogEntry.Level, level);
        }
    }
}

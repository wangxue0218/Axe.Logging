using System;
using Axe.Logging.Core;
using Xunit;

namespace Axe.Logging.Test
{
    public class when_mark_exception_with_logEntry
    {
        [Fact]
        public void should_throw_exception_when_null_call_mark()
        {
            var logEntry = new LogEntry
            {
                Id = Guid.NewGuid(),
                Time = DateTime.UtcNow,
                Entry = "entry uri + httpMethod",
                User = new { CurrentUser = "Lou + pwc001" },
                Data = new { Parameters = "I do not care" },
                Level = LoggingLevel.Info
            };
            var exception = (Exception)null;
            Assert.Throws<ArgumentNullException>(() => exception.Mark(logEntry));
        }

        [Theory]
        [InlineData(LoggingLevel.Info)]
        [InlineData(LoggingLevel.Warn)]
        [InlineData(LoggingLevel.Error)]
        [InlineData(LoggingLevel.Fatal)]
        public void should_get_exception_with_logEntry(LoggingLevel level)
        {
            var logEntry = new LogEntry
            {
                Id = Guid.NewGuid(),
                Time = DateTime.UtcNow,
                Entry = "entry uri + httpMethod",
                User = new {CurrentUser = "Lou + pwc001"},
                Data = new {Parameters = "I do not care"},
                Level = level
            };

            var exception = new Exception();
            exception.Mark(logEntry);

            var axeLogContentOnException = (LogEntry)exception.Data[AxeLoggingExtensions.LogEntryAddToExceptionKey];
            Assert.NotNull(axeLogContentOnException);
            Assert.Equal(axeLogContentOnException.Id, logEntry.Id);
            Assert.Equal(axeLogContentOnException.Time, logEntry.Time);
            Assert.Equal(axeLogContentOnException.Entry, logEntry.Entry);
            Assert.Equal(axeLogContentOnException.User, logEntry.User);
            Assert.Equal(axeLogContentOnException.Data, logEntry.Data);
            Assert.Equal(axeLogContentOnException.Level, level);
        }
    }
}
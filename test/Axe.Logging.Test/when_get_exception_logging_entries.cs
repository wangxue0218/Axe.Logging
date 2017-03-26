using System;
using System.Collections.Generic;
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
            LogEntry logEntry = exception.GetLogEntries().SingleOrDefault();
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
            LogEntry getLogEntry = exception.GetLogEntries().SingleOrDefault();
            Assert.NotNull(getLogEntry);
            Assert.Equal(getLogEntry.Level, level);
        }

        [Fact]
        public void should_return_logging_entry_of_error_level_for_nest_exceptions_of_list_when_inner_exception_not_marked()
        {
            Exception innerException = new Exception();
            for (int i = 0; i < 5; i++)
            {
                innerException = new Exception($"nested exception {i}", innerException);
            }
            LogEntry logEntry = innerException.GetLogEntries().FirstOrDefault();
            Assert.NotNull(logEntry);
            Assert.Equal(LoggingLevel.Error, logEntry.Level);
        }

        [Theory]
        [InlineData(LoggingLevel.Info)]
        [InlineData(LoggingLevel.Warn)]
        [InlineData(LoggingLevel.Error)]
        [InlineData(LoggingLevel.Fatal)]
        public void should_return_logging_entries_for_nest_exceptions_of_list_when_inner_exception_marked(LoggingLevel level)
        {
            Exception innerException = new Exception();
            innerException.Mark(new LogEntry(level));
            for (int i = 0; i < 5; i++)
            {
                innerException = new Exception($"nested exception {i}", innerException);
            }
            LogEntry logEntry = innerException.GetLogEntries().FirstOrDefault();
            Assert.NotNull(logEntry);
            Assert.Equal(level, logEntry.Level);
        }

        [Fact]
        public void should_get_log_entries_when_aggregate_exceptions_marked()
        {
            Exception nestWarnException = new Exception();
            nestWarnException.Mark(new LogEntry(LoggingLevel.Warn));
            for (int i = 0; i < 4; i++)
            {
                nestWarnException = new Exception($"nested exception {i}", nestWarnException);
            }

            Exception nestInfoException = new Exception();
            nestInfoException.Mark(new LogEntry(LoggingLevel.Info));
            for (int i = 0; i < 3; i++)
            {
                nestInfoException = new Exception($"nested exception {i}", nestInfoException);
            }

            var aggregateException = new AggregateException("this is single aggragate exception", nestWarnException, nestInfoException);

            List<LogEntry> logEntries = aggregateException.GetLogEntries();

            Assert.Equal(2, logEntries.Count);
            Assert.Equal(LoggingLevel.Warn, logEntries[0].Level);
            Assert.Equal(LoggingLevel.Info, logEntries[1].Level);
        }

        [Fact]
        public void should_get_log_entries_for_aggregate_exceptions_when_one_exception_is_marked_and_the_other_is_unknow()
        {
            Exception nestWarnException = new Exception();
            nestWarnException.Mark(new LogEntry(LoggingLevel.Warn));
            for (int i = 0; i < 4; i++)
            {
                nestWarnException = new Exception($"nested exception {i}", nestWarnException);
            }
            Exception unknowException = new Exception();
            var aggregateException = new AggregateException("this is single aggragate exception", nestWarnException, unknowException);

            List<LogEntry> logEntries = aggregateException.GetLogEntries();

            Assert.Equal(2, logEntries.Count);
            Assert.Equal(LoggingLevel.Warn, logEntries[0].Level);
            Assert.Equal(LoggingLevel.Error, logEntries[1].Level);
        }

        [Fact]
        public void should_get_log_entries_for_complex_aggregate_exceptions()
        {
            Exception nestWarnException = new Exception();
            nestWarnException.Mark(new LogEntry(LoggingLevel.Warn));
            for (int i = 0; i < 4; i++)
            {
                nestWarnException = new Exception($"nested exception {i}", nestWarnException);
            }
            Exception unknowException = new Exception();
            var childAggregateException = new AggregateException("this is child aggragate exception", nestWarnException, unknowException);

            Exception nestInfoException = new Exception();
            nestInfoException.Mark(new LogEntry(LoggingLevel.Info));
            for (int i = 0; i < 3; i++)
            {
                nestInfoException = new Exception($"nested exception {i}", nestInfoException);
            }
            var aggregateException = new AggregateException("this is root aggragate exception", childAggregateException, nestInfoException);

            List<LogEntry> logEntries = aggregateException.GetLogEntries();

            Assert.Equal(3, logEntries.Count);
            Assert.Equal(LoggingLevel.Info, logEntries[0].Level);
            Assert.Equal(LoggingLevel.Warn, logEntries[1].Level);
            Assert.Equal(LoggingLevel.Error, logEntries[2].Level);
        }
    }
}

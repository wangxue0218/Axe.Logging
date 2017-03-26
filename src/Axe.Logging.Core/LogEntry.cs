using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axe.Logging.Core
{
    [Serializable]
    public class LogEntry
    {
        public Guid Id { get; set; }
        public DateTime Time { get; set; }
        public string Entry { get; set; }
        public object User { get; set; }
        public object Data { get; set; }
        public LoggingLevel Level { get; set; }
    }
}

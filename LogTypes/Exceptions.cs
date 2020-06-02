using AppInsights.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppInsights.Logging
{
    public class Exceptions : LogsBase<Exceptions>
    {
        public string ProblemId { get; set; }
        public string HandledAt { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public string Assembly { get; set; }
        public string Method { get; set; }
        public string OuterType { get; set; }
        public string OuterMessage { get; set; }
        public string OuterAssembly { get; set; }
        public string OuterMethod { get; set; }
        public string InnermostType { get; set; }
        public string InnermostMessage { get; set; }
        public string InnermostAssembly { get; set; }
        public string InnermostMethod { get; set; }
        public int? SeverityLevel { get; set; }
        public dynamic Details { get; set; }

    }
}

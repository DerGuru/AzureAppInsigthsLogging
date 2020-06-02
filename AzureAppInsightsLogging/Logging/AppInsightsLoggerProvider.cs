using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;

namespace AppInsights.Logging
{
    /// <summary>
    /// Used together with ILogger&let;T&gt;
    /// </summary>
    public class AppInsightsLoggerProvider : ILoggerProvider
    {
        private TelemetryClient _tc;

        public AppInsightsLoggerProvider(TelemetryClient tc)
        {
            _tc = tc;
        }
        public ILogger CreateLogger(string categoryName)
            => new AppInsightsLogger(categoryName,  _tc);

        public void Dispose()
        {
        }
    }

}

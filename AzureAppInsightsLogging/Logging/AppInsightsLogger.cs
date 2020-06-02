using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Logging;
using System;

namespace AppInsights.Logging
{
    /// <summary>
    /// Loggs to Application Insights. This class is intended to be used with the ILogger&lt;T&gt; Interface and the LoggingExtensions
    /// in case of an Exception, it logs to "Exceptions" otherwise it logs to CustomEvents
    /// </summary>
    public class AppInsightsLogger : ILogger
    {
        private class Scope<T> : IDisposable
        {
            public Scope(T state)
            {
                State = state;
            }
            public T State { get; set; }
            public void Dispose()
            {
                State = default;
            }
        }

        private string _categoryName;
        private TelemetryClient _telemetryClient;

        /// <summary>
        /// Create an instance of the logger
        /// </summary>
        /// <param name="senderName">name of sender</param>
        /// <param name="telemetryClient">client to send data to Application Insights</param>
        public AppInsightsLogger(string senderName, TelemetryClient telemetryClient)
        {
            _categoryName = senderName;
            _telemetryClient = telemetryClient;
        }

        /// <summary>
        /// scopeing the logging
        /// </summary>
        /// <returns>an IDisposable to keep track of the scope</returns>
        public IDisposable BeginScope<TState>(TState state) => new Scope<TState>(state);

        /// <summary>
        /// always returns true, use Filtering.
        /// </summary>
        /// <param name="logLevel">severity</param>
        /// <returns>true</returns>
        public bool IsEnabled(LogLevel logLevel) => true;

        /// <summary>
        /// Write a logentry to Application Insights
        /// </summary>
        /// <typeparam name="TState">Type of CustomDimension</typeparam>
        /// <param name="logLevel">severity</param>
        /// <param name="eventId">ApplicationInsightsEvent with OperationName</param>
        /// <param name="state">custom dimensions</param>
        /// <param name="exception">an optional exception (null)</param>
        /// <param name="formatter">not used, but formally required</param>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            try
            {
                if (exception != null)
                    LogException(eventId, state, exception);
                else
                {
                    if (logLevel < LogLevel.Information)
                        LogTrace(eventId, state);
                    else
                        LogEvent(eventId, state);
                }
            }
            catch (Exception e)
            {
                _telemetryClient.TrackException(e, new CustomDimensions<Exception>(e));
            }
        }

        /// <summary>
        /// Logs to Traces
        /// </summary>
        /// <typeparam name="TState">DataType of data in CustomDimensions</typeparam>
        /// <param name="eventId">AppInsightsEvent</param>
        /// <param name="state">data for CustomDimensions</param>
        public void LogTrace<TState>(EventId eventId, TState state)
        {
            TraceTelemetry evt = new TraceTelemetry(_categoryName);
            LogTelemetry(eventId, state, evt, _telemetryClient.TrackTrace);
        }

        /// <summary>
        /// Logs to Customevents
        /// </summary>
        /// <typeparam name="TState">DataType of data in CustomDimensions</typeparam>
        /// <param name="eventId">AppInsightsEvent</param>
        /// <param name="state">data for CustomDimensions</param>
        public void LogEvent<TState>(EventId eventId, TState state)
        {
            EventTelemetry evt = new EventTelemetry(_categoryName);
            LogTelemetry(eventId, state, evt, _telemetryClient.TrackEvent);
        }

        /// <summary>
        /// Logs to Exceptions
        /// </summary>
        /// <typeparam name="TState">DataType of data in CustomDimensions</typeparam>
        /// <param name="eventId">AppInsightsEvent</param>
        /// <param name="state">data for CustomDimensions</param>
        /// <param name="exception">caught exception</param>
        public void LogException<TState>(EventId eventId, TState state, Exception exception)
        {
            ExceptionTelemetry evt = new ExceptionTelemetry(exception);
            LogTelemetry(eventId, state, evt, _telemetryClient.TrackException);
        }

        private void LogTelemetry<TState, TelemetryType>(EventId eventId, TState state, TelemetryType telemetry, Action<TelemetryType> trackTelemetry) where TelemetryType : ITelemetry, ISupportProperties
        {
            CustomDimensions data = new CustomDimensions<TState>(state);
            telemetry.Context.Operation.Id = eventId.Id.ToString();
            telemetry.Context.Operation.Name = eventId.Name;
            telemetry.Context.User.Id = (state as IUserDetails)?.UserPrincipalName;
            telemetry.Context.Operation.SyntheticSource = _categoryName;

            foreach (var kvp in data)
                telemetry.Properties.Add(kvp);

            trackTelemetry(telemetry);
        }
    }
}

using Microsoft.Extensions.Logging;

namespace AppInsights.Logging
{
    /// <summary>
    /// Event meta data for the log entry like operation name and potentially event id
    /// </summary>
    public class AppInsightEvent
    {
        /// <summary>
        /// Initialzes a new instance of AppInsightEvent
        /// </summary>
        /// <param name="operationName">name of the operation</param>
        public AppInsightEvent(string operationName) : this(operationName, 0) { }

        /// <summary>
        /// Initialzes a new instance of AppInsightEvent
        /// </summary>
        /// <param name="operationName">name of the operation</param>
        /// <param name="eventId">EventId</param>
        public AppInsightEvent(string operationName, int eventId)
        {
            EventId = eventId;
            OperationName = operationName;
        }

        public int EventId { get; private set; }
        public string OperationName { get; private set; }

        public static implicit operator EventId(AppInsightEvent e) => new EventId(e.EventId, e.OperationName);
        public static implicit operator AppInsightEvent(EventId e) => new AppInsightEvent(e.Name, e.Id);
        public static implicit operator AppInsightEvent(string s) => new AppInsightEvent(s);
    }

    /// <summary>
    /// Collection of the usual suspects (columns) for queries
    /// </summary>
    public enum AppInsightQueryColumns
    {
        name,
        itemType, 
        timestamp, 
        user_Id, 
        operation_Id, 
        operation_Name, 
        operation_SyntheticSource
    }
}

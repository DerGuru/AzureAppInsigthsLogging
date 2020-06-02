namespace AppInsights.Logging
{
    public class CustomEvents : LogsBase<CustomEvents>
    {
        public string Name { get; set; }
        public static string OperationNameEquals(string opName) => $"where {AppInsightQueryColumns.operation_Name} =~ \"{opName}\"";

        public static string OperationNameEquals(AppInsightEvent e) => OperationNameEquals(e.OperationName);
        
    }
}

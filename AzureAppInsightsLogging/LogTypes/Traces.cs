namespace AppInsights.Logging
{
    public class Traces : LogsBase<Traces>
    {
        public string Message { get; set; }
        public int SeverityLevel { get; set; }
    }
}

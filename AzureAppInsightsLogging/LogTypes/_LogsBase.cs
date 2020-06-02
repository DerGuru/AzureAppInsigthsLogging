using System;
using System.Collections.Generic;
using System.Linq;

namespace AppInsights.Logging
{
    public abstract class LogsBase<T> where T : LogsBase<T>, new()
    {
        public static string UserIdEqualsUpn(string upn) => $"where {AppInsightQueryColumns.user_Id} =~ \"{upn}\"";

        public static AppInsightQuery<T> CreateQuery(AppInsightsCredentials apiKey, string query)
            => new AppInsightQuery<T>(apiKey, $"{Decapitalize(typeof(T).Name)} | {query}");

        public static AppInsightQuery<T> CreateQuery(AppInsightsCredentials apiKey, TimeSpan ts, string query) 
            => new AppInsightQuery<T>(apiKey, $"{Decapitalize(typeof(T).Name)} | {query}", ts);

        private static string Decapitalize(string name) => $"{name.First().ToString().ToLowerInvariant()}{new String(name.Skip(1).ToArray())}";
        
        
        public DateTimeOffset Timestamp { get; set; }
        public CustomDimensions CustomDimensions { get; set; }

        public string Operation_Name { get; set; }
        public string Operation_Id { get; set; }
        public string Operation_ParentId { get; set; }
        public string Operation_SyntheticSource { get; set; }
        public dynamic CustomMeasurements { get; set; }
        public string ItemType { get; set; }
        public string Session_Id { get; set; }
        public string User_Id { get; set; }
        public string User_AuthenticatedId { get; set; }
        public string User_AccountId { get; set; }
        public string Application_Version { get; set; }
        public string Client_Type { get; set; }
        public string Client_Model { get; set; }
        public string Client_OS { get; set; }
        public string Client_IP { get; set; }
        public string Client_City { get; set; }
        public string Client_StateOrProvince { get; set; }
        public string Client_CountryOrRegion { get; set; }
        public string Client_Browser { get; set; }
        public string AppId { get; set; }
        public string AppName { get; set; }
        public string IKey { get; set; }
        public string SdkVersion { get; set; }
        public string ItemId { get; set; }
        public int ItemCount { get; set; }
        public string Cloud_RoleName { get; set; }
        public string Cloud_RoleInstance { get; set; }
    }
}

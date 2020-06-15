using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AppInsights
{
    /// <summary>
    /// Query to ApplicationInsights
    /// </summary>
    /// <typeparam name="T">Expected return type</typeparam>
    public class AppInsightQuery<QueryResult>  where QueryResult: class, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppInsightQuery"/> class.
        /// </summary>
        /// <param name="credentials">The credentials for the query</param>
        /// <param name="queryString">The query string.</param>
        public AppInsightQuery(AppInsightsCredentials credentials, string queryString)
        {
            QueryString = queryString;
            Credentials = credentials;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppInsightQuery"/> class.
        /// </summary>
        /// <param name="credentials">The credentials for the query</param>
        /// <param name="queryString">The query string</param>
        /// <param name="querySpan">The query timespan</param>
        public AppInsightQuery(AppInsightsCredentials credentials, string queryString, TimeSpan querySpan) : this (credentials, queryString)
        {
            QuerySpan = querySpan;
        }

        [JsonProperty(PropertyName = "query")]  
        public string QueryString { get; private set; }
        
        [JsonIgnore] 
        public AppInsightsCredentials Credentials { get; private set; }

        [JsonProperty(PropertyName = "timespan", NullValueHandling = NullValueHandling.Ignore)] 
        [JsonConverter(typeof(TimeSpanIsoConverter))] 
        public TimeSpan? QuerySpan { get; private set; } = null;

       
        public async Task<IEnumerable<QueryResult>> LoadFirstTable() 
        {
            var tables = await Post();
            return tables?.FirstOrDefault()?.Rows();
        }

        /// <summary>
        /// Executes the query and returns the resulting tables
        /// </summary>
        /// <returns>the tables in the result</returns>
        public async Task<List<AppInsightsResultBase<QueryResult>.Table>> Post()
        {
            using (HttpClient hc = new HttpClient())
            {
                bool read = false;
                while (!read)
                {
                    try
                    {
                        var req = new HttpRequestMessage(HttpMethod.Post, $"https://api.applicationinsights.io/v1/apps/{Credentials.ApplicationId.ToUnSecureString()}/query");
                        req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        req.Headers.Add("x-api-key", Credentials.ApiKey.ToUnSecureString());
                        req.Content = new StringContent(JsonConvert.SerializeObject(this), Encoding.UTF8, "application/json");
                        var res = await hc.SendAsync(req);
                        var str = await res.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<AppInsightsResultBase<QueryResult>>(str).Tables;
                    }
                    catch (Exception e)
                    {
                        if (e.InnerException != null && e.InnerException is HttpRequestException && e.InnerException.Message.Contains("429"))
                            System.Threading.Thread.Sleep(5000);
                        else
                            throw;
                    }
                }
                return null;
            }
        }

    }

}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppInsights
{
    public class AppInsightsResultBase<T> where T : new()
    {
        public AppInsightsResultBase() { }

        [JsonProperty(PropertyName = "tables")] public List<Table> Tables { get; set; }

        public class Table
        {
            [JsonProperty(PropertyName = "name")]
            public string TableName { get; set; }
            [JsonProperty(PropertyName = "columns")]
            public List<ColumnsDescription> Columns { get; set; }

            [JsonProperty(PropertyName = "rows")]
            public List<List<string>> RawRows { get; set; }

            public IEnumerable<T> Rows() 
            {
                var skeleton = CreateJsonSkeleton();

                LinkedList<T> ll = new LinkedList<T>();
                foreach (var rawRow in RawRows)
                {
                    T t =  Deserialize(skeleton, rawRow);
                    ll.AddLast(t);
                }
                return ll;
            }
            private List<ColumnsDescription> CreateJsonSkeleton()
            {
                var t = typeof(T);
                var props = t.GetProperties();
                return Columns.Select(x => new ColumnsDescription { Name = props.First(y => y.Name.Equals(x.Name, StringComparison.InvariantCultureIgnoreCase)).Name, Type = x.Type }).ToList();
            }

            private static T Deserialize(List<ColumnsDescription> skeleton, List<string> values)
            {
                var json = CreateJson(skeleton, values);
                return JsonConvert.DeserializeObject<T>(json);
            }

            private static readonly string commaAndNewLine = $", {Environment.NewLine}";

            private static string CreateJson(List<ColumnsDescription> skeleton, List<string> values)
            {
                var sb = new StringBuilder();
                var columsAndValues = skeleton
                    .Zip(values,  (x,y) => (x,y))
                    .Select(CreateJsonEntry);

                var innerJson = sb.AppendJoin(commaAndNewLine, columsAndValues);

                return $"{{{Environment.NewLine} {innerJson}{Environment.NewLine}}}";
            }

            private static string CreateJsonEntry((ColumnsDescription First, string Second) x)
                => $"\"{x.First.Name}\" : {x.First.Encapsulation}{x.Second ?? "null"}{x.First.Encapsulation}";
        }

        public class ColumnsDescription
        {
            [JsonProperty(PropertyName = "name")] public string Name { get; set; }
            [JsonProperty(PropertyName = "type")] public string Type { get; set; }

            public String Encapsulation
            {
                get
                {
                    switch (Type)
                    {
                        case "datetime":
                        case "datetimeoffset":
                        case "string":
                            return "\"";

                        default:
                            return "";
                    }
                }
            }

            public override string ToString()
            {
                return $"{Name}:{Type}";
            }

        }
    }
}

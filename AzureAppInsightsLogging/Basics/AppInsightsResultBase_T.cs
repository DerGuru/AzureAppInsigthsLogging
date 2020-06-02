// ***********************************************************************
// Assembly         : EpLogAnalytics
// Author           : JakofHe
// Created          : 11-21-2019
//
// Last Modified By : JakofHe
// Last Modified On : 11-21-2019
// ***********************************************************************
// <copyright file="BaseClass.cs" company="EpLogAnalytics">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppInsights
{
    /// <summary>
    /// Class LogAnalyticsBase.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AppInsightsResultBase<T> where T : new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppInsightsResultBase{T}"/> class.
        /// </summary>
        public AppInsightsResultBase() { }

        /// <summary>
        /// Gets or sets the tables.
        /// </summary>
        /// <value>The tables.</value>
        [JsonProperty(PropertyName = "tables")]
        public List<Table> Tables { get; set; }


        /// <summary>
        /// Class Table.
        /// </summary>
        public class Table
        {
            /// <summary>
            /// Gets or sets the name of the table.
            /// </summary>
            /// <value>The name of the table.</value>
            [JsonProperty(PropertyName = "name")]
            public string TableName { get; set; }
            /// <summary>
            /// Gets or sets the columns.
            /// </summary>
            /// <value>The columns.</value>
            [JsonProperty(PropertyName = "columns")]
            public List<ColumnsDescription> Columns { get; set; }

            /// <summary>
            /// Gets or sets the raw rows.
            /// </summary>
            /// <value>The raw rows.</value>
            [JsonProperty(PropertyName = "rows")]
            public List<List<string>> RawRows { get; set; }

            /// <summary>
            /// Gets the rows.
            /// </summary>
            /// <value>The rows.</value>
            [JsonIgnore]
            public IAsyncEnumerable<T> Rows => ConvertRawRows();

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            private async IAsyncEnumerable<T> ConvertRawRows()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
            {
                var skeleton = CreateJsonSkeleton();

                foreach (var rawRow in RawRows)
                {
                    T t =  Deserialize(skeleton, rawRow);
                    yield return t;
                }
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
                    .Zip(values)
                    .Select(CreateJsonEntry);

                var innerJson = sb.AppendJoin(commaAndNewLine, columsAndValues);

                return $"{{{Environment.NewLine} {innerJson}{Environment.NewLine}}}";
            }

            private static string CreateJsonEntry((ColumnsDescription First, string Second) x)
                => $"\"{x.First.Name}\" : {x.First.Encapsulation}{x.Second ?? "null"}{x.First.Encapsulation}";
        }

        /// <summary>
        /// Class ColumDescription.
        /// </summary>
        public class ColumnsDescription
        {
            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>The name.</value>
            [JsonProperty(PropertyName = "name")]
            public string Name { get; set; }
            /// <summary>
            /// Gets or sets the type.
            /// </summary>
            /// <value>The type.</value>
            [JsonProperty(PropertyName = "type")]
            public string Type { get; set; }

            /// <summary>
            /// Encapsulation for the datatype, derived from the datatype
            /// </summary>
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

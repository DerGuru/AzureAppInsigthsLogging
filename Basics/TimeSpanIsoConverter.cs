// ***********************************************************************
// Assembly         : EpLogAnalytics
// Author           : JakofHe
// Created          : 11-21-2019
//
// Last Modified By : JakofHe
// Last Modified On : 11-21-2019
// ***********************************************************************
// <copyright file="LogAnalyticsQuery.cs" company="EpLogAnalytics">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Newtonsoft.Json;
using System;

namespace AppInsights
{
    /// <summary>
    /// Class TimeSpanIsoConverter.
    /// Implements the <see cref="Newtonsoft.Json.JsonConverter" />
    /// </summary>
    /// <seealso cref="Newtonsoft.Json.JsonConverter" />
    public class TimeSpanIsoConverter : JsonConverter
    {
        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns><c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.</returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string) || objectType == typeof(TimeSpan);
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string s = reader.Value as String;
            return System.Xml.XmlConvert.ToTimeSpan(s);
        }

        /// <summary>
        /// Writes the json.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(System.Xml.XmlConvert.ToString((TimeSpan)value));
        }
    }

}

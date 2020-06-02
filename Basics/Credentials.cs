
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;

namespace AppInsights
{
    /// <summary>
    /// Holds the credentials to access the Logs of the ApplicationInsights (Read Telemetry)
    /// Create an API Key and use it together with the Application ID
    /// </summary>
    public class AppInsightsCredentials 
    {
        [JsonConverter(typeof(SecureStringConverter))] 
        public SecureString ApplicationId { get; set; }
        [JsonConverter(typeof(SecureStringConverter))] 
        public SecureString ApiKey { get; set; }

        public class SecureStringConverter : JsonConverter
        {
            /// <summary>
            /// The types
            /// </summary>
            private static readonly Type[] _types = new Type[] { typeof(SecureString), typeof(string) };

            /// <summary>
            /// Determines whether this instance can convert the specified object type.
            /// </summary>
            /// <param name="objectType">Type of the object.</param>
            /// <returns><c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.</returns>
            public override bool CanConvert(Type objectType) => _types.Contains(objectType);

            /// <summary>
            /// Reads the JSON representation of the object.
            /// </summary>
            /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
            /// <param name="objectType">Type of the object.</param>
            /// <param name="existingValue">The existing value of object being read.</param>
            /// <param name="serializer">The calling serializer.</param>
            /// <returns>The object value.</returns>
            /// <exception cref="InvalidCastException">Unable to cast {reader.ValueType} into SecureString</exception>
            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (CanConvert(reader.ValueType))
                    return (reader.Value as String).ToSecureString();
                else
                    throw new InvalidCastException($"Unable to cast {reader.ValueType} into SecureString");
            }

            /// <summary>
            /// Writes the json.
            /// </summary>
            /// <param name="writer">The writer.</param>
            /// <param name="value">The value.</param>
            /// <param name="serializer">The serializer.</param>
            /// <exception cref="InvalidCastException">Unable to cast {value.GetType()} as SecureString into String</exception>
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                if (CanConvert(value.GetType()))
                    writer.WriteValue((value as SecureString).ToUnSecureString());
                else
                    throw new InvalidCastException($"Unable to cast {value.GetType()} as SecureString into String");
            }
        }
    }
    internal static class StringExtensions
    {
        /// <summary>
        /// Converts this string to it's secure equivalent
        /// </summary>
        /// <param name="unsecure">The input string</param>
        /// <returns>a secure string containing the characters from the unsecure string</returns>
        public static SecureString ToSecureString(this string unsecure)
        {
            SecureString secure = new SecureString();
            unsecure.ToList().ForEach(c => secure.AppendChar(c));
            return secure;
        }

        /// <summary>
        /// Converts this secure string to it's unsecure equivalent
        /// </summary>
        /// <param name="secure">The input secure string</param>
        /// <returns>an unsecure string, containing the characters from the secure string</returns>
        public static string ToUnSecureString(this SecureString secure) =>
            Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(secure));
    }
}
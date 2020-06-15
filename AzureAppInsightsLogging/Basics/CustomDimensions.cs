using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AppInsights.Logging
{
    public class CustomDimensions : Dictionary<string, string>, IDictionary<string, JToken>
    {
       
        #region Creation
        /// <summary>
        /// no empty constructor for you
        /// </summary>
        protected CustomDimensions() { }

        /// <summary>
        /// copy constructor
        /// </summary>
        /// <param name="cd"></param>
        public CustomDimensions(CustomDimensions cd)
        {
            CopyAllKeysAndValues(cd);
        }

        public CustomDimensions(IDictionary<string, JToken> inputData) : this(inputData.ToDictionary(x => x.Key, y => y.Value.ToString())) { }

        public CustomDimensions(IDictionary<string, string> inputData)
        {
            if (IsNotWrittenWithLogger(inputData))
                AddTheWholeDictionaryAsJson(inputData);
            else
                CopyAllKeysAndValues(inputData);
        }
        private static bool IsNotWrittenWithLogger(IDictionary<string, string> inputData) =>
            !(inputData.ContainsKey(nameof(Type)) || inputData.ContainsKey("Data"));


        private void AddTheWholeDictionaryAsJson(IDictionary<string, string> inputData)
        {
            Type = typeof(Dictionary<string, string>);
            this["Data"] = JsonConvert.SerializeObject(inputData.ToDictionary(x => x.Key, y => y.Value));
        }

        private void CopyAllKeysAndValues(IDictionary<string, string> inputData)
        {
            foreach (var kvp in inputData)
            {
                this[kvp.Key] = kvp.Value;
            }
        }

        #endregion

        public Type Type
        {
            get => JsonConvert.DeserializeObject<Type>(this[nameof(Type)]);
            set => this[nameof(Type)] = JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// "down cast" to the generic variant
        /// </summary>
        /// <typeparam name="T">Type of the inner Data</typeparam>
        /// <returns>a CustomDimension of T</returns>
        public CustomDimensions<T> Load<T>() => new CustomDimensions<T>(this);

        /// <summary>
        /// Get Data as an object
        /// </summary>
        /// <returns></returns>
        public object Data => JsonConvert.DeserializeObject(this["Data"], Type);

        /// <summary>
        /// Get typed data
        /// </summary>
        /// <typeparam name="T">Type of expected inner data</typeparam>
        /// <exception cref="TypeLoadException">Throws if types are not a match</exception>
        /// <returns>data</returns>
        public T DataAs<T>() where T : class => JsonConvert.DeserializeObject<T>(this["Data"]);

        /// <summary>
        /// Is the DataType (derived) of this generic type
        /// </summary>
        /// <returns>true if "Type" is this generic or derived from it. False otherwise</returns>
        public bool TypesAreMatching<T>() => TypesAreMatching<T>(Data);

        /// <summary>
        /// Is the DataType (derived) of this generic type
        /// </summary>
        /// <returns>true if "Type" is this generic or derived from it. False otherwise</returns>
        private bool TypesAreMatching<T>(object data) => typeof(T).IsInstanceOfType(data);

        /// <summary>
        /// returns the Data-JSON 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (!ContainsKey("Data"))
                return $"Missing Key \"Data\".";
            else
                return this["Data"];
        }

        #region IDictionary
        void IDictionary<string, JToken>.Add(string key, JToken value)
            => Add(key, value.ToString());

        bool IDictionary<string, JToken>.ContainsKey(string key)
            => ContainsKey(key);

        bool IDictionary<string, JToken>.Remove(string key) => Remove(key);

        bool IDictionary<string, JToken>.TryGetValue(string key, out JToken value)
        {
            string val;
            var res = TryGetValue(key, out val);
            if (res)
                value = JToken.Parse(val);
            else
                value = null;
            return res;
        }

        void ICollection<KeyValuePair<string, JToken>>.Add(KeyValuePair<string, JToken> item)
            => Add(item.Key, item.Value.ToString());

        void ICollection<KeyValuePair<string, JToken>>.Clear()
            => Clear();

        bool ICollection<KeyValuePair<string, JToken>>.Contains(KeyValuePair<string, JToken> item)
        {
            if (ContainsKey(item.Key))
            {
                var val = item.Value.ToString();
                return this[item.Key] == val;
            }
            return false;
        }

        void ICollection<KeyValuePair<string, JToken>>.CopyTo(KeyValuePair<string, JToken>[] array, int arrayIndex)
        {
            (this as Dictionary<string, string>)
                .Select(x => new KeyValuePair<string, JToken>(x.Key, JToken.Parse(x.Value)))
                .ToList()
                .CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<string, JToken>>.Remove(KeyValuePair<string, JToken> item)
        {
            if ((this as ICollection<KeyValuePair<string, JToken>>).Contains(item))
                return Remove(item.Key);
            else
                return false;
        }

        IEnumerator<KeyValuePair<string, JToken>> IEnumerable<KeyValuePair<string, JToken>>.GetEnumerator()
        {
            return (this as Dictionary<string, string>)
                .Select(x => new KeyValuePair<string, JToken>(x.Key, JToken.Parse(x.Value))).GetEnumerator();
        }

        ICollection<string> IDictionary<string, JToken>.Keys => this.Keys;

        ICollection<JToken> IDictionary<string, JToken>.Values => this.Values.Select(x => JToken.Parse(x)).ToList();

        int ICollection<KeyValuePair<string, JToken>>.Count => this.Count;

        bool ICollection<KeyValuePair<string, JToken>>.IsReadOnly => true;

        JToken IDictionary<string, JToken>.this[string key]
        {
            get => JToken.Parse(this[key]);
            set => this[key] = value.ToString();
        }
        #endregion
    }
}

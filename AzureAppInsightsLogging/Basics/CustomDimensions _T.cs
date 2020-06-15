using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AppInsights.Logging
{

    public class CustomDimensions<T> : CustomDimensions
    {
        
        public CustomDimensions(CustomDimensions other) : base(other)
        {
            if (!TypesAreMatching<T>())
                throw new TypeLoadException($"{Type.FullName} is not of {typeof(T).FullName}!");
        }

        //Construct from Data
        public CustomDimensions(T data)
        {
            Data = data;
            _data = new Lazy<T>(() => JsonConvert.DeserializeObject<T>(this[nameof(Data)]));
        }

        /// <summary>
        /// Json wants this to deserialize the whole thing
        /// </summary>
        /// <param name="d"></param>
        public CustomDimensions(IDictionary<string, JToken> d) : this(d.ToDictionary(x => x.Key, y => y.Value.ToString()))
        {
        }

        /// <summary>
        /// Application Insights expects and delivers an IDictionary&lt;string,string&gt>
        /// </summary>
        /// <param name="d"></param>
        public CustomDimensions(IDictionary<string, string> d) : base(d)
        {
            if (TypesAreMatching<T>())
                _data = new Lazy<T>(() => JsonConvert.DeserializeObject<T>(this[nameof(Data)]));
            else
                throw new TypeLoadException($"{Type.FullName} is not of {typeof(T).FullName}!");
        }


        private Lazy<T> _data;

       
        public new T Data
        {
            get => _data.Value;

            set
            {
                Type = value?.GetType() ?? typeof(T);
                this[nameof(Data)] = JsonConvert.SerializeObject(value);
            }
        }
    }
}

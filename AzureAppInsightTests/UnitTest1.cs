using AppInsights;
using AppInsights.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Reflection.Emit;

namespace AzureAppInsightTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void GoodCase()
        {
            using (StreamReader sr = new StreamReader("data.json"))
            {
                var json = sr.ReadToEnd();
                var tables = JsonConvert.DeserializeObject<AppInsightsResultBase<CustomEvents>>(json).Tables;
                foreach (var row in tables.First().Rows())
                {
                    var test = row.CustomDimensions.DataAs<string>();
                    Assert.AreEqual("testString", test);
                }
            }
        }

        [TestMethod]
        public void BaseType()
        {
            using (StreamReader sr = new StreamReader("data.json"))
            {
                var json = sr.ReadToEnd();
                var tables = JsonConvert.DeserializeObject<AppInsightsResultBase<CustomEvents>>(json).Tables;
                foreach (var row in tables.First().Rows())
                {
                    var test = row.CustomDimensions.DataAs<object>();
                    Assert.AreEqual("testString", test.ToString());
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(JsonSerializationException))]
        public void WrongType()
        {
            using (StreamReader sr = new StreamReader("data.json"))
            {
                var json = sr.ReadToEnd();
                var tables = JsonConvert.DeserializeObject<AppInsightsResultBase<CustomEvents>>(json).Tables;
                foreach (var row in tables.First().Rows())
                {
                    var test = row.CustomDimensions.DataAs<CustomAttributeBuilder>();
                }
            }
        }
    }
}

using AppInsights;
using AppInsights.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace AzureAppInsightTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task GoodCase()
        {
            using (StreamReader sr = new StreamReader("data.json"))
            {
                var json = sr.ReadToEnd();
                var tables = JsonConvert.DeserializeObject<AppInsightsResultBase<CustomEvents>>(json).Tables;
                await foreach (var row in tables.First().Rows)
                {
                    var test = row.CustomDimensions.DataAs<string>();
                    Assert.AreEqual("testString", test);
                }
            }
        }

        [TestMethod]
        public async Task BaseType()
        {
            using (StreamReader sr = new StreamReader("data.json"))
            {
                var json = sr.ReadToEnd();
                var tables = JsonConvert.DeserializeObject<AppInsightsResultBase<CustomEvents>>(json).Tables;
                await foreach (var row in tables.First().Rows)
                {
                    var test = row.CustomDimensions.DataAs<object>();
                    Assert.AreEqual("testString", test.ToString());
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(TypeLoadException))]
        public async Task WrongType()
        {
            using (StreamReader sr = new StreamReader("data.json"))
            {
                var json = sr.ReadToEnd();
                var tables = JsonConvert.DeserializeObject<AppInsightsResultBase<CustomEvents>>(json).Tables;
                await foreach (var row in tables.First().Rows)
                {
                    var test = row.CustomDimensions.DataAs<CustomAttributeBuilder>();
                }
            }
        }
    }
}

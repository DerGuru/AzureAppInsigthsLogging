using AppInsights;
using AppInsights.Logging;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

        public async static Task<AppInsightsCredentials> LoadFromKeyVaultAsync(string baseUrl, string secretName, string version)
        {
            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
            KeyVaultClient keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            var secret = await keyVaultClient.GetSecretAsync(baseUrl, secretName, version);
            return  JsonConvert.DeserializeObject<AppInsightsCredentials>(secret.Value);
        }
    }
}

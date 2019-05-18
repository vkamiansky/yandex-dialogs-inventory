using System;
using Xunit;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace TestApi.IntegrationTests
{
    public class UnitTest1
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public UnitTest1()
        {
            _server = new TestServer(WebHost.CreateDefaultBuilder().UseStartup<TestApi.Startup>());
            _client = _server.CreateClient();
        }

        [Fact]
        public async Task Test1()
        {
            var result = await _client.GetAsync("/api/values");
            var stringResult = await result.Content.ReadAsStringAsync();
            Assert.Equal("[\"value1\",\"value2\"]",stringResult);
        }
    }
}

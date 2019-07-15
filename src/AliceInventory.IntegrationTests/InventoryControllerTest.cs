using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AliceInventory.Controllers;
using AliceInventory.Logic;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;

namespace AliceInventory.IntegrationTests
{
    public class InventoryControllerTest
    {
        public InventoryControllerTest()
        {
            _server = new TestServer(WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>()
                .ConfigureTestServices(services =>
                {
                    services.AddSingleton<IConfigurationService, TestConfigurationService>();
                }));
            _client = _server.CreateClient();
        }

        private static readonly Meta MetaExample = new Meta
        {
            Locale = "ru-RU",
            Timezone = "Europe/Moscow",
            ClientId = "ru.yandex.searchplugin/5.80 (Samsung Galaxy; Android 4.4)"
        };

        private readonly TestServer _server;
        private readonly HttpClient _client;

        private async Task<AliceResponse> SendRequest(HttpClient client, AliceRequest request)
        {
            var requestJson = JsonConvert.SerializeObject(request);
            var requestContent = new StringContent(requestJson, Encoding.Default, "application/json");

            var responseContent = await client.PostAsync("/api/inventory/alice", requestContent);

            var responseJson = await responseContent.Content.ReadAsStringAsync();

            try
            {
                return JsonConvert.DeserializeObject<AliceResponse>(responseJson);
            }
            catch (JsonReaderException)
            {
                return null;
            }
        }

        [Fact]
        public async Task JsonVerification()
        {
            var request = new AliceRequest
            {
                Meta = MetaExample,
                Request = new Request
                {
                    Command = "не пустой текст",
                    OriginalUtterance = "Не пустой текст",
                    Type = RequestType.SimpleUtterance
                },
                Session = new Session
                {
                    New = true,
                    MessageId = 0,
                    SessionId = "2eac4854-fce721f3-b845abba-20d60",
                    SkillId = "3ad36498-f5rd-4079-a14b-788652932056",
                    UserId = "AC9WC3DF6FCE052E45A4566A48E6B7193774B84814CE49A922E163B8B29881DC"
                },
                Version = "1.0"
            };

            var responseModel = await SendRequest(_client, request);

            // Json test
            Assert.NotNull(responseModel);

            // Required properties test
            Assert.NotNull(responseModel?.Response);
            Assert.NotNull(responseModel?.Response?.Text);
            Assert.NotNull(responseModel?.Response?.EndSession);
            Assert.NotNull(responseModel?.Session);
            Assert.NotNull(responseModel?.Session?.SessionId);
            Assert.NotNull(responseModel?.Session?.MessageId);
            Assert.NotNull(responseModel?.Session?.UserId);
            Assert.NotNull(responseModel?.Version);

            // Session test
            Assert.Equal(request.Session.SessionId, responseModel?.Session?.SessionId);
            Assert.Equal(request.Session.MessageId, responseModel?.Session?.MessageId);
            Assert.Equal(request.Session.UserId, responseModel?.Session?.UserId);
        }

        [Fact]
        public async Task ServerAvailableTest()
        {
            var request = new HttpRequestMessage(HttpMethod.Head, "/api/inventory/");
            using (var response = await _client.SendAsync(request))
            {
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task ServerRunningTest()
        {
            using (var response = await _client.GetAsync("/api/inventory/"))
            {
                var responseString = await response.Content.ReadAsStringAsync();
                Assert.Contains("Server is working...", responseString);
            }
        }
    }
}
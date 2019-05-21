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
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using AliceInventory;
using Newtonsoft.Json;

namespace AliceInventory.IntegrationTests
{
    public class PostTest
    {
        private static readonly TimeSpan TimeLimit = TimeSpan.FromSeconds(1.5f);
        private static readonly MetaModel MetaExample = new MetaModel()
        {
            Locale = "ru-RU",
            Timezone = "Europe/Moscow",
            ClientId = "ru.yandex.searchplugin/5.80 (Samsung Galaxy; Android 4.4)"
        };

        private readonly TestServer _server;
        private readonly HttpClient _client;

        public PostTest()
        {
            _server = new TestServer(WebHost.CreateDefaultBuilder().UseStartup<AliceInventory.Startup>());
            _client = _server.CreateClient();
        }

        //private string jsonExample = "{\"meta\":{\"locale\":\"ru-RU\",\"timezone\":\"Europe/Moscow\",\"client_id\": \"ru.yandex.searchplugin/5.80 (Samsung Galaxy; Android 4.4)\",    \"interfaces\": {      \"screen\": { }    }  },  \"request\": {    \"command\": \"*TEXT*\",    \"original_utterance\": \"закажи пиццу на улицу льва толстого, 16 на завтра\",    \"type\": \"SimpleUtterance\",    \"markup\": {      \"dangerous_context\": true    },    \"payload\": {},    \"nlu\": {      \"tokens\": [        \"закажи\",        \"пиццу\",        \"на\",        \"льва\",        \"толстого\",        \"16\",        \"на\",        \"завтра\"      ],      \"entities\": [        {          \"tokens\": {            \"start\": 2,            \"end\": 6          },          \"type\": \"YANDEX.GEO\",          \"value\": {            \"house_number\": \"16\",            \"street\": \"льва толстого\"          }        },        {          \"tokens\": {            \"start\": 3,            \"end\": 5          },          \"type\": \"YANDEX.FIO\",          \"value\": {            \"first_name\": \"лев\",            \"last_name\": \"толстой\"          }        },        {          \"tokens\": {            \"start\": 5,            \"end\": 6          },          \"type\": \"YANDEX.NUMBER\",          \"value\": 16        },        {          \"tokens\": {            \"start\": 6,            \"end\": 8          },          \"type\": \"YANDEX.DATETIME\",          \"value\": {            \"day\": 1,            \"day_is_relative\": true          }        }      ]    }  },  \"session\": {    \"new\": true,    \"message_id\": 4,    \"session_id\": \"2eac4854-fce721f3-b845abba-20d60\",    \"skill_id\": \"3ad36498-f5rd-4079-a14b-788652932056\",    \"user_id\": \"AC9WC3DF6FCE052E45A4566A48E6B7193774B84814CE49A922E163B8B29881DC\"  },  \"version\": \"1.0\"}";

        private async Task<AliceResponse> SendRequest(HttpClient client, AliceRequest request)
        {
            var requestJson = JsonConvert.SerializeObject(request);
            var requestContent = new StringContent(requestJson, Encoding.Default, "application/json");

            var startTime = DateTime.Now;
            var responseContent = await client.PostAsync("/api/values", requestContent);
            var endTime = DateTime.Now;
            Assert.InRange(endTime - startTime, TimeSpan.Zero, TimeLimit);

            var responseJson = await responseContent.Content.ReadAsStringAsync();

            try
            {
                return JsonConvert.DeserializeObject<AliceResponse>(responseJson);
            }
            catch (JsonReaderException e)
            {
                return null;
            }
        }
        
        [Fact]
        public async Task JsonVerification()
        {
            var request = new AliceRequest()
            {
                Meta = MetaExample,
                Request = new RequestModel()
                {
                    Command = "не пустой текст",
                    OriginalUtterance = "Не пустой текст",
                    Type = RequestType.SimpleUtterance
                },
                Session = new SessionModel()
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
    }
}

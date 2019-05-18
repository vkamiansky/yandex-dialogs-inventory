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
using System.Text;
using System.Threading.Tasks;
using AliceInventory;
using Newtonsoft.Json;
using Xunit.Extensions.Ordering;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
[assembly: TestCaseOrderer("Xunit.Extensions.Ordering.TestCaseOrderer", "Xunit.Extensions.Ordering")]
[assembly: TestCollectionOrderer("Xunit.Extensions.Ordering.CollectionOrderer", "Xunit.Extensions.Ordering")]
namespace AliceInventory.IntegrationTests
{
    [Order(1)]
    public class PostTest
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public PostTest()
        {
            _server = new TestServer(WebHost.CreateDefaultBuilder().UseStartup<AliceInventory.Startup>());
            _client = _server.CreateClient();
        }

        //private string jsonExample = "{\"meta\":{\"locale\":\"ru-RU\",\"timezone\":\"Europe/Moscow\",\"client_id\": \"ru.yandex.searchplugin/5.80 (Samsung Galaxy; Android 4.4)\",    \"interfaces\": {      \"screen\": { }    }  },  \"request\": {    \"command\": \"*TEXT*\",    \"original_utterance\": \"закажи пиццу на улицу льва толстого, 16 на завтра\",    \"type\": \"SimpleUtterance\",    \"markup\": {      \"dangerous_context\": true    },    \"payload\": {},    \"nlu\": {      \"tokens\": [        \"закажи\",        \"пиццу\",        \"на\",        \"льва\",        \"толстого\",        \"16\",        \"на\",        \"завтра\"      ],      \"entities\": [        {          \"tokens\": {            \"start\": 2,            \"end\": 6          },          \"type\": \"YANDEX.GEO\",          \"value\": {            \"house_number\": \"16\",            \"street\": \"льва толстого\"          }        },        {          \"tokens\": {            \"start\": 3,            \"end\": 5          },          \"type\": \"YANDEX.FIO\",          \"value\": {            \"first_name\": \"лев\",            \"last_name\": \"толстой\"          }        },        {          \"tokens\": {            \"start\": 5,            \"end\": 6          },          \"type\": \"YANDEX.NUMBER\",          \"value\": 16        },        {          \"tokens\": {            \"start\": 6,            \"end\": 8          },          \"type\": \"YANDEX.DATETIME\",          \"value\": {            \"day\": 1,            \"day_is_relative\": true          }        }      ]    }  },  \"session\": {    \"new\": true,    \"message_id\": 4,    \"session_id\": \"2eac4854-fce721f3-b845abba-20d60\",    \"skill_id\": \"3ad36498-f5rd-4079-a14b-788652932056\",    \"user_id\": \"AC9WC3DF6FCE052E45A4566A48E6B7193774B84814CE49A922E163B8B29881DC\"  },  \"version\": \"1.0\"}";

        private StringContent CreateJsonContent(string text)
        {
            AliceRequest response = new AliceRequest() { Request = new RequestModel() { Command = text }};
            string json = JsonConvert.SerializeObject(response);
            return new StringContent(json, Encoding.Default, "application/json");
        }
        
        [Theory, Order(1)]
        [InlineData("Добавь 3 яблока", new []{ "Теперь яблок 3 шт" })]
        [InlineData("Добавь 1 яблоко", new []{ "Теперь яблок 4 шт" })]
        [InlineData("Ещё 7 груши", new []{ "Теперь груш 7 шт" })]
        [InlineData("Прибавь 8 литров воды", new []{ "Теперь воды 8 литров" })]
        public async Task Adding(string request, string[] responces)
        {
            var content = CreateJsonContent(request);
            var result = await _client.PostAsync("/api/values", content);
            var jsonResult = await result.Content.ReadAsStringAsync();
            var aliceAnswer = JsonConvert.DeserializeObject<AliceResponse>(jsonResult).Response.Text;

            Assert.Contains(responces, expected => expected == aliceAnswer);
        }

        [Theory, Order(2)]
        [InlineData("Убери 2 яблока", new []{ "Теперь яблок 2 шт" })]
        [InlineData("Вычти 1 грушу", new []{ "Теперь груш 6 шт" })]
        [InlineData("Убери 1 литр воды", new []{ "Теперь воды 7 литров" })]
        public async Task Removing(string request, string[] responces)
        {
            var content = CreateJsonContent(request);
            var result = await _client.PostAsync("/api/values", content);
            var jsonResult = await result.Content.ReadAsStringAsync();
            var aliceAnswer = JsonConvert.DeserializeObject<AliceResponse>(jsonResult).Response.Text;

            Assert.Contains(responces, expected => expected == aliceAnswer);
        }
        [Theory, Order(3)]
        [InlineData("Теперь яблок 3", new []{ "Теперь яблок 3 шт" })]
        [InlineData("Теперь груш 5", new []{ "Теперь груш 5 шт" })]
        [InlineData("Теперь воды 3 литра", new []{ "Теперь воды 3 л" })]
        [InlineData("Теперь песка 3 килограмма", new []{ "Теперь песка 3 кг" })]
        public async Task Setting(string request, string[] responces)
        {
            var content = CreateJsonContent(request);
            var result = await _client.PostAsync("/api/values", content);
            var jsonResult = await result.Content.ReadAsStringAsync();
            var aliceAnswer = JsonConvert.DeserializeObject<AliceResponse>(jsonResult).Response.Text;

            Assert.Contains(responces, expected => expected == aliceAnswer);
        }

        [Theory, Order(4)]
        [InlineData("Сколько песка", new []{ "Песок: 3 кг" })]
        [InlineData("Сколько воды", new []{ "Вода: 3 л" })]
        [InlineData("Сколько яблок", new []{ "Яблоки: 3 шт" })]
        public async Task Showing(string request, string[] responces)
        {
            var content = CreateJsonContent(request);
            var result = await _client.PostAsync("/api/values", content);
            var jsonResult = await result.Content.ReadAsStringAsync();
            var aliceAnswer = JsonConvert.DeserializeObject<AliceResponse>(jsonResult).Response.Text;

            Assert.Contains(responces, expected => expected == aliceAnswer);
        }

        [Theory, Order(5)]
        [InlineData("Покажи отчёт", new []{ "Отчёт:\nЯблоки: 3 шт\nГруши: 5 шт\nВода: 3 л\nПесок: 3 кг" })]
        public async Task ShowingAll(string request, string[] responces)
        {
            var content = CreateJsonContent(request);
            var result = await _client.PostAsync("/api/values", content);
            var jsonResult = await result.Content.ReadAsStringAsync();
            var aliceAnswer = JsonConvert.DeserializeObject<AliceResponse>(jsonResult).Response.Text;

            Assert.Contains(responces, expected => expected == aliceAnswer);
        }
    }
}

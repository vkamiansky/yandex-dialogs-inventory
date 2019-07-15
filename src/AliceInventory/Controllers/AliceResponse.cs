using Newtonsoft.Json;

namespace AliceInventory.Controllers
{
    public class AliceResponse
    {
        [JsonProperty("response")] public Response Response { get; set; }

        [JsonProperty("session")] public Session Session { get; set; }

        [JsonProperty("version")] public string Version { get; set; } = "1.0";
    }
}
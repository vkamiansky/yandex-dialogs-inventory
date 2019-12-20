using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AliceInventory.Controllers
{
    public enum RequestType
    {
        SimpleUtterance,
        ButtonPressed
    }
    public class Request
    {
        [JsonProperty("command")]
        public string Command { get; set; }

        [JsonProperty("type")]
        public RequestType Type { get; set; }

        [JsonProperty("original_utterance")]
        public string OriginalUtterance { get; set; }

        [JsonProperty("payload")]
        public string Payload { get; set; }
    }
}

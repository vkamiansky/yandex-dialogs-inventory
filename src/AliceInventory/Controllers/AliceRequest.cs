using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AliceInventory.Controllers
{
    public class AliceRequest
    {
        [JsonProperty("meta")]
        public Meta Meta { get; set; }

        [JsonProperty("request")]
        public Request Request { get; set; }

        [JsonProperty("session")]
        public Session Session { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }
    }
}

using Newtonsoft.Json;

namespace AliceInventory.Controllers
{
    public class Meta
    {
        [JsonProperty("locale")] public string Locale { get; set; }

        [JsonProperty("timezone")] public string Timezone { get; set; }

        [JsonProperty("client_id")] public string ClientId { get; set; }
    }
}
using Newtonsoft.Json;

namespace AliceInventory.Controllers
{
    public class Response
    {
        [JsonProperty("text")] public string Text { get; set; }

        [JsonProperty("tts")] public string Tts { get; set; }

        [JsonProperty("end_session")] public bool EndSession { get; set; }

        [JsonProperty("buttons")] public Button[] Buttons { get; set; }
    }
}
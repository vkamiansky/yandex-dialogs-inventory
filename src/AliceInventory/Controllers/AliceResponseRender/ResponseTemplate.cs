namespace AliceInventory.Controllers.AliceResponseRender
{
    public class ResponseTemplate
    {
        public TextAndSpeechTemplate[] TextAndSpeechTemplates { get; set; }
        public Button[] Buttons { get; set; }
        public bool EndSession { get; set; }
    }
}
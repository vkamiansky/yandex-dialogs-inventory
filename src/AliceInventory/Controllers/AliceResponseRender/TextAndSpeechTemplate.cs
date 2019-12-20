using System;

namespace AliceInventory.Controllers.AliceResponseRender
{
    public class TextAndSpeechTemplate
    {
        public string TextTemplate { get; }
        public string SpeechTemplate { get; }

        public TextAndSpeechTemplate(string text)
        {
            TextTemplate = text;
            SpeechTemplate = text;
        }

        public TextAndSpeechTemplate(string text, string tts)
        {
            TextTemplate = text;
            SpeechTemplate = tts;
        }

        public string FormatText(IFormatProvider provider, params object[] parts)
        {
            return string.Format(provider, TextTemplate, parts);
        }
        
        public string FormatSpeech(IFormatProvider provider, params object[] parts)
        {
            return string.Format(provider, SpeechTemplate, parts);
        }
    }
}
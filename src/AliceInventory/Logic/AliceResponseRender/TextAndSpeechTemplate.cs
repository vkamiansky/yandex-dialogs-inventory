using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AliceInventory.Logic.AliceResponseRender
{
    
    class TextAndSpeechTemplate
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

        public TextAndSpeech Format(params object[] parts)
        {
            return new TextAndSpeech()
            {
                Text = string.Format(TextTemplate, parts),
                Speech = string.Format(SpeechTemplate, parts)
            };
        }
    }
}
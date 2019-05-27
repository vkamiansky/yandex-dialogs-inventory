using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AliceInventory.Controllers;

namespace AliceInventory.Logic.AliceResponseRender
{
    public class ResponseTemplate
    {
        public TextAndSpeechTemplate[] TextAndSpeechTemplates { get; set; } 
        public Button[] Buttons { get; set; }
    }
}

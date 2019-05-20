using System.Collections.Generic;
using System.Text;
using System.Linq;
using System;

namespace ConsoleApp
{
    class Parser
    {
        public ChatResponse TryParse(string userText)
        {
            //some analysis of userText
            //...
            return new ChatResponse(ChatResponse.UserAction.Add, "someItem", 10, new Unit("kg"));
        }
    }
}

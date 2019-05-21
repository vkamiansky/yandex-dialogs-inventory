using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp
{
    public class UserSession
    {
        Stack<ChatResponse> responsesHistory;
        static Parser parser = new Parser();
        public ChatResponse ProcessInput(string input)
        {
            ChatResponse response = new ChatResponse(parser.TryParse(input));
            responsesHistory.Push(response);
            return new ChatResponse(response);
        }
    }
}

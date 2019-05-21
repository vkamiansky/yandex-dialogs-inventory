using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp
{
    using static ChatResponse;
    public class UserSession
    {
        Stack<ChatResponse> responsesHistory;
        Parser parser = new Parser();
        IDataStorage userStorage = new Storage();

        public ChatResponse ProcessInput(string input)
        {
            ChatResponse response = new ChatResponse(parser.TryParse(input));

            switch(response.userAction)
            {
                case UserAction.Add:
                    userStorage.Add(response);
                    break;
                case UserAction.clearAll:
                    userStorage.ClearAll();
                    break;
                case UserAction.deleteLast:
                    userStorage.DeleteEntry(responsesHistory.Pop());
                    break;
                case UserAction.readAll:
                    response = MakeAllEntriesResponse(userStorage.ReadAll());
                    break;
            }

            responsesHistory.Push(response);
            return response;
        }

        private ChatResponse MakeAllEntriesResponse(HashSet<Entry> allEntries)
        {
            //some work to transform HashSet to ChatResponse
            return new ChatResponse();
        }
    }
}

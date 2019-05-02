using System.Collections.Generic;
using System.Text;
using System.Linq;
using System;

namespace ConsoleApp {
    public class UserSession {

        private Parser Parser { get; set; }

        private List<ParserResponse> ItemList { get; set; }

        private Stack<string> history =new Stack<string>();

        public UserSession() {
            Parser = new Parser();
            ItemList =  new List<ParserResponse>();
        }

        public ChatResponse ProcessInput(string input)
         {
            return new ChatResponse() { TextResponse = "Test responce", VoiceResponse = "Voice responce" };

             var clearedInput=input
                .Replace("Учёт материалов", "")
                .Replace("учёт материалов", "")
                .Replace("Учет материалов", "")
                .Replace("учет материалов", "")
                .TrimStart(' ')
                .TrimStart(Parser.Separators);

            var command = TryGetCommand(clearedInput);
            history.Push(clearedInput);
            var response = new ChatResponse();

            switch (command) {
                case "добавить":
                case "add":
                    response = AddItem(clearedInput.Replace(command, string.Empty));
                    break;
                case "покажи":
                case "показать":
                case "выведи":
                case "вывести":
                case "список":
                case "show":
                case "list":
                    response = ShowList();
                    break;
                case "очистить":
                case "сбросить":
                case "clear":
                case "шухер":
                case "reset":
                case "подтвердить":
                case "подтверждаю":
                case "да":
                    response = ClearList();
                    break;
                default:
                    response.TextResponse = "команда не распознана";
                    response.VoiceResponse = "команда не распознана";
                    break;
            }

            history.Push(command);
            return response;
        }

        private ChatResponse AddItem(string input) 
        {            
            var response = Parser.TryParse(input);
            if (response != null)
            {
                if(ItemList.Any(x=>string.Equals(
                    x.ItemString, 
                    response.ItemString, 
                    StringComparison.CurrentCultureIgnoreCase )))   
                {
                    ItemList.First(x=>string.Equals(
                    x.ItemString, 
                    response.ItemString, 
                    StringComparison.CurrentCultureIgnoreCase)).ItemCount+=response.ItemCount;
                }           
                else
                {
                    ItemList.Add(response);
                }
                return new ChatResponse() 
                {
                    TextResponse = $"добавили {response.ItemString} - {response.ItemCount} {response.Unit.ShortName}",
                    VoiceResponse = "добавлено",
                };
            }
            return null;
        }

        private ChatResponse ShowList() 
        {
            var sb = new StringBuilder();
            int i=1;
            foreach (var item in ItemList) {
                sb.Append($"{i++}. {item.ItemString} - {item.ItemCount} {item.Unit}\n");
            }
            return new ChatResponse() {
                TextResponse = sb.ToString(),
                VoiceResponse = "Выведено на экран",
            };
        }

        private ChatResponse ClearList() 
        {
            switch(history.Peek())
            {
                case "очистить":
                case "сбросить":
                case "clear":
                case "шухер":
                case "reset":
                    ItemList.Clear();
                    return new ChatResponse() 
                    {
                        TextResponse = "список очищен",
                        VoiceResponse = "список очищен",
                    };
                default: 
                    return new ChatResponse() 
                    {
                        TextResponse = "Подтвердите очистку списка",
                        VoiceResponse = "Подтвердите очистку списка",
                    };
            }
        }

        private string TryGetCommand(string input) {
            var words = input.Split(" ");
            return words.Length > 0
            ? words[0]
            : string.Empty;
        }
    }
}
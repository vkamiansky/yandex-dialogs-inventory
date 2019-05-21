
namespace ConsoleApp
{
    public class ChatResponse: Entry {

        public string TextResponse { get; set; }
        public string VoiceResponse { get; set; }
        public UserAction userAction { get; set; }

        public enum UserAction
        {
            Add,
            deleteLast,
            readAll,
            clearAll,
            exit
        }

        public ChatResponse()
        {

        }

        // Copy constructor
        public ChatResponse(ChatResponse newChatResponse): 
            this(newChatResponse.userAction, newChatResponse.ItemName, 
            newChatResponse.ItemCount, newChatResponse.Unit)
        {
        }

        // Instance constructor
        public ChatResponse(UserAction action, string itemName, double itemCount, Unit unit)
        {
            userAction = action;
            base.ItemName = itemName;
            base.ItemCount = itemCount;
            base.Unit = unit;
        }
    }
}
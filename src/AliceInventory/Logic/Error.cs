namespace AliceInventory.Logic
{
    public class Error
    {
        public string Message { get; }

        public Error()
        { }

        public Error(string message)
        { Message = message; }
    }
}

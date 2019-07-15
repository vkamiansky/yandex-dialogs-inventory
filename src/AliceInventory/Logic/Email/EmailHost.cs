namespace AliceInventory.Logic.Email
{
    public class EmailHost
    {
        public EmailHost(string url, int port)
        {
            Url = url;
            Port = port;
        }

        public string Url { get; set; }

        public int Port { get; set; }
    }
}
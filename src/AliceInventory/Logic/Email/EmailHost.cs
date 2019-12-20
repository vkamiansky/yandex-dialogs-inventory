namespace AliceInventory.Logic.Email
{
    public class EmailHost
    {
        public string Url { get; set; }
        public int Port { get; set; }
        public EmailHost(string url, int port)
        {
            Url = url;
            Port = port;
        }
    }
}

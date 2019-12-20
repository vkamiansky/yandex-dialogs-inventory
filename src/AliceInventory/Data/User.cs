namespace AliceInventory.Data
{
    public class User
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public User(string id)
        {
            Id = id;
        }
    }
}
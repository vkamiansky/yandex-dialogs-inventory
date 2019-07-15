namespace AliceInventory.Logic.Email
{
    public interface IInventoryEmailService
    {
        void SendListAsync(string email, Entry[] entries);
    }
}
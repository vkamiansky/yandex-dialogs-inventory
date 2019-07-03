namespace AliceInventory.Data
{
    public interface IUserDataStorage
    {
        int CreateEntry(string userId, string entryName, double quantity, Data.UnitOfMeasure unit);
        Data.Entry[] ReadAllEntries(string userId);
        void UpdateEntry(int id, double quantity);
        void DeleteEntry(int id);
        void DeleteAllEntries(string userId);

        string ReadUserEmail(string userId);
        void SetUserEmail(string userId, string email);
        string DeleteUserEmail(string userId);
    }
}
using System;

namespace AliceInventory.Data
{
    public interface IUserDataStorage
    {
        Guid CreateEntry(string userId, string entryName, double quantity, Data.UnitOfMeasure unit);
        Data.Entry[] ReadAllEntries(string userId);
        void UpdateEntry(Guid id, double quantity);
        void DeleteEntry(Guid id);
        void DeleteAllEntries(string userId);
        string ReadUserMail(string userId);
        void SetUserMail(string userId, string email);
        void DeleteUserMail(string userId);
    }
}
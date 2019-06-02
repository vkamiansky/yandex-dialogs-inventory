using System;

namespace AliceInventory.Data
{
    public interface IUserDataStorage
    {
        bool AddEntry(string userId, SingleEntry item);
        bool DeleteEntry(string userId, SingleEntry item);
        Data.Entry[] ReadAllEntries(string userId);
        bool ClearInventory(string userId);
        string GetUserEmail(string userId);
        bool SetUserEmail(string userId, string email);
        string DeleteUserEmail(string userId);
    }
}

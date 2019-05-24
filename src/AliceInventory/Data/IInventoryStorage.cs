using System;

namespace AliceInventory.Data
{
    public interface IInventoryStorage
    {
        bool Add(string userId, Entry item);
        bool Delete(string userId, Entry item);
        Entry[] ReadAll(string userId);
        bool Clear(string userId);
    }
}

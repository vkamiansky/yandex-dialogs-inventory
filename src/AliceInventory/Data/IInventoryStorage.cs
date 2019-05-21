using System;

namespace AliceInventory.Data
{
    public interface IInventoryStorage
    {
        void Add(string userId, Entry item);
        void Delete(string userId, Entry item);
        Entry[] ReadAll(string userId);
        void Clear(string userId);
    }
}

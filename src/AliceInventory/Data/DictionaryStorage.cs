using System;

namespace AliceInventory.Data
{
    public class DictionaryStorage : IInventoryStorage
    {
        public void Add(string userId, Entry item)
        {
            throw new NotImplementedException();
        }

        public void Delete(string userId, Entry item)
        {
            throw new NotImplementedException();
        }

        public Entry[] ReadAll(string userId)
        {
            throw new NotImplementedException();
        }

        public void Clear(string userId)
        {
            throw new NotImplementedException();
        }
    }
}

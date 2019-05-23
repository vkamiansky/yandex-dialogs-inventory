using System;
using System.Collections.Generic;

namespace AliceInventory.Data
{
    public class DictionaryStorage : IInventoryStorage
    {
        private Dictionary<string, Dictionary<string, HashSet<Entry>>> storage;

        public DictionaryStorage()
        {
            storage = new Dictionary<string, Dictionary<string, HashSet<Entry>>>();
        }

        public void Add(string userId, Entry item)
        {
            if (storage.ContainsKey(userId))
            {

            }
            else
            {
                storage.Add(userId, new Dictionary<string, HashSet<Entry>>());
            }
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

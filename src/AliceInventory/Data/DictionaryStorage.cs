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
                if (storage[userId].ContainsKey(item.Name))
                {

                }
                else
                {
                    HashSet<Entry> entries = new HashSet<Entry>();
                    entries.Add(item);
                    storage[userId].Add(item.Name, entries);
                }
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

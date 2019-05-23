using System;
using System.Collections.Generic;

namespace AliceInventory.Data
{
    public class DictionaryStorage : IInventoryStorage
    {
        private Dictionary<string, Dictionary<string, Dictionary<UnitOfMeasure, Entry>>> storage;

        public DictionaryStorage()
        {
            storage = new Dictionary<string, Dictionary<string, Dictionary<UnitOfMeasure, Entry>>>();
        }

        public void Add(string userId, Entry item)
        {
            if (!storage.ContainsKey(userId))
                storage.Add(userId, new Dictionary<string, Dictionary<UnitOfMeasure, Entry>>());

            var userEntries = storage[userId];

            if (!userEntries.ContainsKey(item.Name))
                userEntries.Add(item.Name, new Dictionary<UnitOfMeasure, Entry>());

            userEntries[item.Name].Add(item.Unit, item);
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

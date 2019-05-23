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
            var entriesItem = userEntries[item.Name];

            if (!entriesItem.ContainsKey(item.Unit))
                entriesItem.Add(item.Unit, item);
            else
                entriesItem[item.Unit].Count += item.Count;
        }

        public void Delete(string userId, Entry item)
        {
            if (storage.ContainsKey(userId) &&
                storage[userId].ContainsKey(item.Name) &&
                storage[userId][item.Name].ContainsKey(item.Unit))
            {
                Entry deletingItem = storage[userId][item.Name][item.Unit];
                deletingItem.Count -= item.Count;

                if (deletingItem.Count <= 0)
                    storage[userId][item.Name].Remove(item.Unit);
            }
        }

        public Entry[] ReadAll(string userId)
        {
            throw new NotImplementedException();
        }

        public void Clear(string userId)
        {
            if (storage.ContainsKey(userId))
                storage[userId].Clear();
        }
    }
}

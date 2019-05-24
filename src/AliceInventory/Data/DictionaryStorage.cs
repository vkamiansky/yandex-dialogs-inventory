using System;
using System.Linq;
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

        public bool Add(string userId, Entry item)
        {
            bool isSuccessful = true;

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

            return isSuccessful;
        }

        public bool Delete(string userId, Entry item)
        {
            bool isSuccessful = true;

            if (storage.ContainsKey(userId) &&
                storage[userId].ContainsKey(item.Name) &&
                storage[userId][item.Name].ContainsKey(item.Unit))
            {
                Entry deletingItem = storage[userId][item.Name][item.Unit];
                deletingItem.Count -= item.Count;

                if (deletingItem.Count <= 0)
                    storage[userId][item.Name].Remove(item.Unit);
            }
            else
                isSuccessful = false;

            return isSuccessful;
        }

        public Entry[] ReadAll(string userId)
        {
            if (!storage.ContainsKey(userId))
                return null;

            Entry[] entries = storage[userId].SelectMany(userEntries => userEntries.Value.Values).ToArray();
            
            return entries;
        }

        public bool Clear(string userId)
        {
            bool isSuccessful = true;

            if (storage.ContainsKey(userId))
                storage[userId].Clear();
            else
                isSuccessful = false;

            return isSuccessful;
        }
    }
}

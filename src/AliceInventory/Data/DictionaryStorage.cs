using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace AliceInventory.Data
{
    public class DictionaryStorage : IUserDataStorage
    {
        private Dictionary<string, UserData> storage;

        public DictionaryStorage()
        {
            storage = new Dictionary<string, UserData>();
        }

        public bool AddEntry(string userId, SingleEntry item)
        {
            bool isSuccessful = true;

            UserData data = GetUserData(userId);
            var entries = data.Entries;

            if (entries.All(x => x.Name != item.Name))
                entries.Add(new Entry(item.Name));

            var userItem = entries.First(x => x.Name == item.Name);

            if (!userItem.UnitValues.ContainsKey(item.Unit))
                userItem.UnitValues.Add(item.Unit, item.Count);
            else
                userItem.UnitValues[item.Unit] += item.Count;

            return isSuccessful;
        }

        public bool DeleteEntry(string userId, SingleEntry item)
        {
            UserData data = GetUserData(userId);
            var entries = data.Entries;
            var userItem = entries.FirstOrDefault(x => x.Name == item.Name);

            if (userItem == null) return false;
            if (!userItem.UnitValues.ContainsKey(item.Unit)) return false;

            // Removing
            userItem.UnitValues[item.Unit] -= item.Count;
            
            // Data cleaning
            if (userItem.UnitValues[item.Unit] > 0) return true;
            userItem.UnitValues.Remove(item.Unit);
            
            if (userItem.UnitValues.Count > 0) return true;
            entries.Remove(userItem);
            return true;
        }

        public Data.Entry[] ReadAllEntries(string userId)
        {
            UserData data = GetUserData(userId);
            return data.Entries.ToArray();
        }

        public bool ClearInventory(string userId)
        {
            bool isSuccessful = true;

            UserData data = GetUserData(userId);
            data.Entries.Clear();

            return isSuccessful;
        }

        public string GetUserEmail(string userId)
        {
            UserData data = GetUserData(userId);
            return data.LastEmail;
        }

        public bool SetUserEmail(string userId, string email)
        {
            bool isSuccessful = true;

            UserData data = GetUserData(userId);
            data.LastEmail = email;

            return isSuccessful;
        }

        public string DeleteUserEmail(string userId)
        {
            UserData data = GetUserData(userId);
            var email = data.LastEmail;
            data.LastEmail = null;

            return email;
        }


        private UserData GetUserData(string userId)
        {
            if (!storage.ContainsKey(userId))
                storage.Add(userId, new UserData());
                
            return storage[userId];
        }
    }
}

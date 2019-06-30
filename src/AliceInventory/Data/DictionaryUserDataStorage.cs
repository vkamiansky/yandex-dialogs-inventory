using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using AliceInventory.Data.Exceptions;

namespace AliceInventory.Data
{
    public class DictionaryUserDataStorage : IUserDataStorage
    {
        private Dictionary<string, UserData> storage;

        public DictionaryUserDataStorage()
        {
            storage = new Dictionary<string, UserData>();
        }

        public void AddEntry(string userId, string entryName, double count, Data.UnitOfMeasure unit)
        {
            UserData data = GetUserData(userId);
            var entries = data.Entries;

            if (entries.All(x => x.Name != entryName))
                entries.Add(new Entry(entryName));

            var userItem = entries.First(x => x.Name == entryName);

            if (!userItem.UnitValues.ContainsKey(unit))
                userItem.UnitValues.Add(unit, count);
            else
                userItem.UnitValues[unit] += count;
        }

        public void DeleteEntry(string userId, string entryName, double count, Data.UnitOfMeasure unit)
        {
            UserData data = GetUserData(userId);
            var entries = data.Entries;
            var userItem = entries.FirstOrDefault(x => x.Name == entryName);

            if (userItem == null)
                throw new EntryNotFoundException(userId, entryName);
            if (!userItem.UnitValues.ContainsKey(unit))
                throw new EntryUnitNotFoundException(userId, userItem, unit);

            var currentCount = userItem.UnitValues[unit];
            // Removing
            if (currentCount < count)
                throw new NotEnoughEntryToDeleteException(userId, count, currentCount, userItem);

            userItem.UnitValues[unit] -= count;
            
            // Data cleaning
            if (userItem.UnitValues[unit] <= 0) return;
            userItem.UnitValues.Remove(unit);
            
            if (userItem.UnitValues.Count > 0) return;
            entries.Remove(userItem);
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

using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using AliceInventory.Data.Errors;
using AliceInventory.Logic;

namespace AliceInventory.Data
{
    public class DictionaryUserDataStorage : IUserDataStorage
    {
        private Dictionary<string, UserData> storage;

        public DictionaryUserDataStorage()
        {
            storage = new Dictionary<string, UserData>();
        }

        public OperationResult AddEntry(string userId, string entryName, double count, Data.UnitOfMeasure unit)
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

            return OperationResult.Ok;
        }

        public OperationResult DeleteEntry(string userId, string entryName, double count, Data.UnitOfMeasure unit)
        {
            UserData data = GetUserData(userId);
            var entries = data.Entries;
            var userItem = entries.FirstOrDefault(x => x.Name == entryName);

            if (userItem == null)
                return new EntryNotFoundError(userId, entryName);
            if (!userItem.UnitValues.ContainsKey(unit))
                return new EntryUnitNotFoundError(userId, userItem, unit);

            var currentCount = userItem.UnitValues[unit];
            // Removing
            if (currentCount < count)
                return new NotEnoughEntryToDeleteError(userId, count, currentCount, userItem);

            userItem.UnitValues[unit] -= count;
            
            // Data cleaning
            if (userItem.UnitValues[unit] <= 0)
                return OperationResult.Ok;
            userItem.UnitValues.Remove(unit);
            
            if (userItem.UnitValues.Count > 0)
                return OperationResult.Ok;
            entries.Remove(userItem);

            return OperationResult.Ok;
        }

        public OperationResult<Data.Entry[]> ReadAllEntries(string userId)
        {
            UserData data = GetUserData(userId);
            return data.Entries.ToArray();
        }

        public OperationResult ClearInventory(string userId)
        {
            UserData data = GetUserData(userId);
            data.Entries.Clear();

            return OperationResult.Ok;
        }

        public OperationResult<string> GetUserEmail(string userId)
        {
            UserData data = GetUserData(userId);
            return data.LastEmail;
        }

        public OperationResult SetUserEmail(string userId, string email)
        {
            UserData data = GetUserData(userId);
            data.LastEmail = email;

            return OperationResult.Ok;
        }

        public OperationResult<string> DeleteUserEmail(string userId)
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

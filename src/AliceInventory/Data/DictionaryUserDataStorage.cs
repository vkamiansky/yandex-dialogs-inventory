using System;
using System.Linq;
using System.Collections.Generic;
using AliceInventory.Logic;

namespace AliceInventory.Data
{
    public class DictionaryUserDataStorage : IUserDataStorage
    {
        private static int _lastEntryId = 0;
        private readonly HashSet<Data.Entry> _entries;
        private readonly Dictionary<string, string> _userEmails;

        public DictionaryUserDataStorage()
        {
            _entries = new HashSet<Entry>();
            _userEmails = new Dictionary<string, string>();
        }


        public int CreateEntry(string userId, string entryName, double quantity, UnitOfMeasure unit)
        {
            var entry = new Data.Entry()
            {
                Id = _lastEntryId++,
                Name = entryName,
                Quantity = quantity,
                UnitOfMeasure = unit,
                UserId = userId
            };
            _entries.Add(entry);
            return entry.Id;
        }

        public void DeleteEntry(int id)
        {
            _entries.RemoveWhere(e => e.Id == id);
        }

        public void UpdateEntry(int id, double quantity)
        {
            _entries.First(e => e.Id == id).Quantity = quantity;
        }

        public Data.Entry[] ReadAllEntries(string userId)
        {
            return _entries.Where(e => e.UserId == userId).ToArray();
        }

        public void DeleteAllEntries(string userId)
        {
            _entries.RemoveWhere(e => e.UserId == userId);
        }

        public string ReadUserEmail(string userId)
        {
            return _userEmails.ContainsKey(userId) ? _userEmails[userId] : null;
        }

        public void SetUserEmail(string userId, string email)
        {
            if (_userEmails.ContainsKey(userId))
                _userEmails[userId] = email;
            else
                _userEmails.Add(userId, email);
        }

        public string DeleteUserEmail(string userId)
        {
            var email = _userEmails[userId];
            _userEmails.Remove(userId);
            return email;
        }
    }
}

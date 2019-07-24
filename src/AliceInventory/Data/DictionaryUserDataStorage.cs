using System.Linq;
using System.Collections.Generic;
using System;

namespace AliceInventory.Data
{
    public class DictionaryUserDataStorage : IUserDataStorage
    {
        private readonly HashSet<Data.Entry> _entries;
        private readonly Dictionary<string, string> _userEmails;

        public DictionaryUserDataStorage()
        {
            _entries = new HashSet<Entry>();
            _userEmails = new Dictionary<string, string>();
        }

        public Guid CreateEntry(string userId, string entryName, double quantity, UnitOfMeasure unit)
        {
            var entry = new Data.Entry()
            {
                Id = Guid.NewGuid(),
                Name = entryName,
                Quantity = quantity,
                UnitOfMeasure = unit,
                UserId = userId
            };
            _entries.Add(entry);
            return entry.Id;
        }

        public void DeleteEntry(Guid id)
        {
            _entries.RemoveWhere(e => e.Id == id);
        }

        public void UpdateEntry(Guid id, double quantity)
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

        public string ReadUserMail(string userId)
        {
            return _userEmails.ContainsKey(userId) ? _userEmails[userId] : null;
        }

        public void SetUserMail(string userId, string email)
        {
            if (_userEmails.ContainsKey(userId))
                _userEmails[userId] = email;
            else
                _userEmails.Add(userId, email);
        }

        public void DeleteUserMail(string userId)
        {
            _userEmails.Remove(userId);
        }
    }
}

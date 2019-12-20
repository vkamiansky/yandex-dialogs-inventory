using System;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace AliceInventory.Data
{
    public class MongoUserDataStorage : IUserDataStorage
    {
        private readonly IMongoDatabase _database;

        private IMongoCollection<User> _users => _database.GetCollection<User>("Users");

        private IMongoCollection<Entry> _entries => _database.GetCollection<Entry>("Entries");

        public MongoUserDataStorage(Logic.IConfigurationService configuration)
        {
            var client = new MongoClient(configuration.GetDbConnectionString().Result);
            _database = client.GetDatabase(configuration.GetDbName().Result);
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
            _entries.InsertOne(entry);
            return entry.Id;
        }

        public void DeleteEntry(Guid id)
        {
            _entries.DeleteOne(e => e.Id == id);
        }

        public void UpdateEntry(Guid id, double quantity)
        {
            var builder = new UpdateDefinitionBuilder<Entry>();
            var update = builder.Set(x => x.Quantity, quantity);
            _entries.UpdateOne(e => e.Id == id, update);
        }

        public Data.Entry[] ReadAllEntries(string userId)
        {
            return _entries
            .AsQueryable()
            .Where(e => e.UserId == userId)
            .ToArray();
        }

        public void DeleteAllEntries(string userId)
        {
            _entries.DeleteMany(e => e.UserId == userId);
        }

        public string ReadUserMail(string userId)
        {
            return _users.AsQueryable()
            .FirstOrDefault(u => u.Id == userId)?.Email;
        }

        public void SetUserMail(string userId, string email)
        {
            if (!_users.AsQueryable().Any(u => u.Id == userId))
                _users.InsertOne(new User(userId));

            var builder = new UpdateDefinitionBuilder<User>();
            var update = builder.Set(x => x.Email, email);
            _users.UpdateOne(e => e.Id == userId, update);
        }

        public void DeleteUserMail(string userId)
        {
            var builder = new UpdateDefinitionBuilder<User>();
            var update = builder.Set(x => x.Email, null);
            _users.UpdateOne(e => e.Id == userId, update);
        }
    }
}
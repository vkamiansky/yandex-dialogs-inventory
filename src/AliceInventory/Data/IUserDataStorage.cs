using System;
using AliceInventory.Logic;

namespace AliceInventory.Data
{
    public interface IUserDataStorage
    {
        OperationResult AddEntry(string userId, string entryName, double count, Data.UnitOfMeasure unit);
        OperationResult DeleteEntry(string userId, string entryName, double count, Data.UnitOfMeasure unit);
        OperationResult<Data.Entry[]> ReadAllEntries(string userId);
        OperationResult ClearInventory(string userId);
        OperationResult<string> GetUserEmail(string userId);
        OperationResult SetUserEmail(string userId, string email);
        OperationResult<string> DeleteUserEmail(string userId);
    }
}

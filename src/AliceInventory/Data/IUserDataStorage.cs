namespace AliceInventory.Data
{
    public interface IUserDataStorage
    {
        OperationResult<int> AddEntry(string userId, string entryName, double quantity, Data.UnitOfMeasure unit);
        OperationResult DeleteEntry(int id);
        OperationResult UpdateEntry(int id, double quantity);
        OperationResult<Data.Entry[]> ReadAllEntries(string userId);
        OperationResult DeleteAllEntries(string userId);
        OperationResult<string> GetUserEmail(string userId);
        OperationResult SetUserEmail(string userId, string email);
        OperationResult<string> DeleteUserEmail(string userId);
    }
}
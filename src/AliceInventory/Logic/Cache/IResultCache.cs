namespace AliceInventory.Logic.Cache
{
    public interface IResultCache
    {
        void Set(string userId, ProcessingResult command);
        ProcessingResult Get(string userId);
    }
}
using System;

namespace AliceInventory.Logic
{
    public interface ICommandCache
    {
        void Set(string userId, ProcessingCommand command);
        ProcessingCommand Get(string userId);
    }
}

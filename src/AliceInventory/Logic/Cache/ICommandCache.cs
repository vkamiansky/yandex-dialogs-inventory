using System;
using AliceInventory.Logic.Parser;

namespace AliceInventory.Logic
{
    public interface ICommandCache
    {
        void Set(string userId, ProcessingResult command);
        ProcessingResult Get(string userId);
    }
}

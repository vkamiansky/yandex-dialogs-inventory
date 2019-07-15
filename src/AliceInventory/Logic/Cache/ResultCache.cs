using System.Collections.Generic;

namespace AliceInventory.Logic.Cache
{
    public class ResultCache : IResultCache
    {
        private readonly Dictionary<string, ProcessingResult> usersCommands;

        public ResultCache()
        {
            usersCommands = new Dictionary<string, ProcessingResult>();
        }

        public ProcessingResult Get(string userId)
        {
            return usersCommands.ContainsKey(userId) ? usersCommands[userId] : null;
        }

        public void Set(string userId, ProcessingResult command)
        {
            if (usersCommands.ContainsKey(userId))
                usersCommands[userId] = command;
            else
                usersCommands.Add(userId, command);
        }
    }
}
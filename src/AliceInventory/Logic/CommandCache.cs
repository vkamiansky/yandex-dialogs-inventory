using System;
using System.Collections.Generic;

namespace AliceInventory.Logic
{
    public class CommandCache : ICommandCache
    {
        private Dictionary<string, ProcessingResult> usersCommands;

        public CommandCache()
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

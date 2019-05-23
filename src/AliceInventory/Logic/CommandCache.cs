using System;
using System.Collections.Generic;

namespace AliceInventory.Logic
{
    public class CommandCache : ICommandCache
    {
        private Dictionary<string, ProcessingCommand> usersCommands;

        public CommandCache()
        {
            usersCommands = new Dictionary<string, ProcessingCommand>();
        }

        public ProcessingCommand Get(string userId)
        {
            return usersCommands.ContainsKey(userId) ? usersCommands[userId] : null;
        }

        public void Set(string userId, ProcessingCommand command)
        {
            throw new NotImplementedException();
        }
    }
}

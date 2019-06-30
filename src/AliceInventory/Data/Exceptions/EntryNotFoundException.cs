using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AliceInventory.Data.Exceptions
{
    public class EntryNotFoundException : Exception
    {
        public string UserId { get; }
        public string EntryName { get; }

        public EntryNotFoundException(string userId, string entryName)
        : base($"{entryName} not found in database for user {userId}")
        {
            UserId = userId;
            EntryName = entryName;
        }
    }
}

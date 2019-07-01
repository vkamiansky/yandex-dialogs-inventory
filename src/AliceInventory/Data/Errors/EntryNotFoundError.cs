using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AliceInventory.Logic;

namespace AliceInventory.Data.Errors
{
    public class EntryNotFoundError : Error
    {
        public string UserId { get; }
        public string EntryName { get; }

        public EntryNotFoundError(string userId, string entryName)
        : base($"{entryName} not found in database for user {userId}")
        {
            UserId = userId;
            EntryName = entryName;
        }
    }
}

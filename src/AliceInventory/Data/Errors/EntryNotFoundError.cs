using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AliceInventory.Logic;

namespace AliceInventory.Data.Errors
{
    public class EntryNotFoundError : StorageError
    {
        public string EntryName { get; }

        public EntryNotFoundError(string userId, string entryName)
        : base($"{entryName} not found in database")
        {
            EntryName = entryName;
        }
    }
}

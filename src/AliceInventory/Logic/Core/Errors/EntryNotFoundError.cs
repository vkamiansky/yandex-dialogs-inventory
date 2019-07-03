using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AliceInventory.Logic.Core.Errors
{
    public class EntryNotFoundError : Error
    {
        public string EntryName { get; }

        public EntryNotFoundError(string userId, string entryName)
            : base($"{entryName} not found in database")
        {
            EntryName = entryName;
        }
    }
}

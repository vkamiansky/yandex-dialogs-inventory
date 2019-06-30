using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AliceInventory.Logic;

namespace AliceInventory.Data.Exceptions
{
    public class EntryUnitNotFoundException : Exception
    {
        public string UserId { get; }
        public Entry Entry { get; }
        public UnitOfMeasure Unit { get; }

        public EntryUnitNotFoundException(string userId, Entry entry, UnitOfMeasure unit)
            : base($"{unit} not found for {entry.Name} in database for user {userId}")
        {
            UserId = userId;
            Entry = entry;
            Unit = unit;
        }
    }
}

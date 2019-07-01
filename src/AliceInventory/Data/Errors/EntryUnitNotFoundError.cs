using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AliceInventory.Logic;

namespace AliceInventory.Data.Errors
{
    public class EntryUnitNotFoundError : StorageError
    {
        public Entry Entry { get; }
        public UnitOfMeasure Unit { get; }

        public EntryUnitNotFoundError(string userId, Entry entry, UnitOfMeasure unit)
            : base($"{unit} not found for {entry.Name} in database")
        {
            Entry = entry;
            Unit = unit;
        }
    }
}

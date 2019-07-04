using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AliceInventory.Logic.Core.Errors
{
    public class EntryNotFoundInDatabaseError : Error
    {
        public string EntryName { get; }
        public UnitOfMeasure EntryUnit { get; }

        public EntryNotFoundInDatabaseError(string entryName, UnitOfMeasure unit)
            : base($"{entryName} not found in database")
        {
            EntryName = entryName;
            EntryUnit = unit;
        }
    }
}

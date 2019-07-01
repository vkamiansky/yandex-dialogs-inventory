using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AliceInventory.Logic;

namespace AliceInventory.Data.Errors
{
    public class NotEnoughEntryToDeleteError : StorageError
    {
        public double Actual { get; }
        public double Count { get; }
        public Entry Entry { get; }

        public NotEnoughEntryToDeleteError(string userId, double actual, double count, Entry entry)
            : base($"Can't delete {actual} from {count} entry(ies)")
        {
            Actual = actual;
            Count = count;
            Entry = entry;
        }
    }
}

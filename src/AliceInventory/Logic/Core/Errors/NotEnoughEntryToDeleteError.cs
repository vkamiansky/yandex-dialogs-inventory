using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AliceInventory.Logic.Core.Errors
{
    public class NotEnoughEntryToDeleteError : Error
    {
        public double Actual { get; }
        public Data.Entry DataEntry { get; }

        public NotEnoughEntryToDeleteError(double actual, Data.Entry dataEntry)
            : base($"Can't delete {actual} from {dataEntry.Quantity} entry(ies)")
        {
            Actual = actual;
            DataEntry = dataEntry;
        }
    }
}

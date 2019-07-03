using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AliceInventory.Logic
{
    public class Entry
    {
        public string Name { get; set; }
        public double Quantity { get; set; }
        public UnitOfMeasure UnitOfMeasure { get; set; }
    }
}

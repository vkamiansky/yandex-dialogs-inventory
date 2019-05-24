using System;

namespace AliceInventory.Data
{
    public class Entry
    {
        public string Name { get; set; }
        public double Count { get; set; }
        public UnitOfMeasure Unit { get; set; }
    }
}

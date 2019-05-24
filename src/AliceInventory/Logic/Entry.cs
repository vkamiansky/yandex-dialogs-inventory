using System;

namespace AliceInventory.Logic
{
    public class Entry
    {
        public string Name { get; set; }
        public double Count { get; set; }
        public UnitOfMeasure Unit { get; set; }

        public Entry()
        {

        }

        public Entry(string name, double count) 
        {
            Name = name;
            Count = count;
        }

        public Entry(string name, double count, UnitOfMeasure unit) : this(name, count)
        {
            Unit = unit;
        }
    }
}

using System;
using System.Collections.Generic;

namespace AliceInventory.Data
{
    public class Entry
    {
        public string Name { get; set; }
        public Dictionary<UnitOfMeasure, double> UnitValues { get; set; }

        public Entry(string name)
        {
            Name = name;
            UnitValues = new Dictionary<UnitOfMeasure, double>();
        }
    }
}

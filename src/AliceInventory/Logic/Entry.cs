using System;
using System.Collections.Generic;

namespace AliceInventory.Logic
{
    public class Entry
    {
        public string Name { get; set; }
        public Dictionary<Logic.UnitOfMeasure, double> UnitValues { get; set; }
        public Entry(string name)
        {
            Name = name;
            UnitValues = new Dictionary<Logic.UnitOfMeasure, double>();
        }

        public Entry(string name, double count, Logic.UnitOfMeasure unit)
        {
            Name = name;
            UnitValues = new Dictionary<UnitOfMeasure, double> { {unit, count} };
        }
    }
}

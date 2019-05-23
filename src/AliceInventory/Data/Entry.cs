using System;

namespace AliceInventory.Data
{
    public class Entry
    {
        public string Name { get; set; }
        public double Count { get; set; }
        public UnitOfMeasure Unit { get; set; }

        public Entry(string name, double count)
        {
            Name = name;
            Count = count;
        }

        public Entry(string name, double count, UnitOfMeasure unit) : this(name, count)
        {
            Unit = unit;
        }

        public override bool Equals(object obj)
        {
            Entry that = obj as Entry;

            if (that == null)
                return false;

            return Name.Equals(that.Name) &&
                Unit.Equals(that.Unit);
        }

        public override int GetHashCode()
        {
            int coef = 7;
            int hash = 1;
            hash = (hash * coef) + Name != null ? Name.GetHashCode() : 0;
            hash = (hash * coef) + Unit.GetHashCode();
            return hash;
        }
    }
}

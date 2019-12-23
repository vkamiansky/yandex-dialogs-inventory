using System;
using AliceInventory.Logic.Parser;

namespace AliceInventory.Data
{
    public class Entry
    {
        private string _name;
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string Name { get => _name; set => _name = WordNormalizer.Normalize(value); }
        public UnitOfMeasure UnitOfMeasure { get; set; }
        public double Quantity { get; set; }
    }
}

using AliceInventory.Logic.Parser;
using EP.Ner;
using EP.Ner.Core;
using EP.Morph;

namespace AliceInventory.Logic
{
    public class Entry
    {
        private string name;
        public string Name
        {
            get => name;
            set => name = WordNormalizer.Normalize(value);
        }
        public double Quantity { get; set; }
        public UnitOfMeasure UnitOfMeasure { get; set; }
    }
}

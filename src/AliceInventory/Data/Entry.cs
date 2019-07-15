namespace AliceInventory.Data
{
    public class Entry
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public UnitOfMeasure UnitOfMeasure { get; set; }
        public double Quantity { get; set; }
    }
}
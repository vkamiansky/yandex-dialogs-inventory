namespace AliceInventory.Logic.Core.Errors
{
    public class EntryNotFoundInDatabaseError : Error
    {
        public EntryNotFoundInDatabaseError(string name, UnitOfMeasure unit)
            : base($"{name} not found in database")
        {
            EntryName = name;
            EntryUnit = unit;
        }

        public string EntryName { get; }
        public UnitOfMeasure EntryUnit { get; }
    }
}
namespace AliceInventory.Logic.Core.Errors
{
    public class EntryNotFoundInDatabaseError : Error
    {
        public string EntryName { get; }
        public Logic.UnitOfMeasure EntryUnit { get; }

        public EntryNotFoundInDatabaseError(string name, Logic.UnitOfMeasure unit)
            : base($"{name} not found in database")
        {
            EntryName = name;
            EntryUnit = unit;
        }
    }
}

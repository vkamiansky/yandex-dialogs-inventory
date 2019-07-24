namespace AliceInventory.Logic.Core.Errors
{
    public class NotEnoughEntryToDeleteError : Error
    {
        public string EntryName { get; }
        public double Actual { get; }
        public double Expected { get; }

        public NotEnoughEntryToDeleteError(string name, double expected, double actual)
            : base($"Can't delete {expected} from {actual} entry(ies)")
        {
            Actual = actual;
            Expected = expected;
            EntryName = name;
        }
    }
}

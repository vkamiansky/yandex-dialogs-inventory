using System.Globalization;

namespace AliceInventory.Logic
{
    public class UserInput
    {
        public string Raw { get; set; }
        public string Prepared { get; set; }
        public string Button { get; set; }
        public CultureInfo CultureInfo { get; set; }
    }
}
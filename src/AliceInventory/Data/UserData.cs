using System.Collections.Generic;

namespace AliceInventory.Data
{
    public class UserData
    {
        public HashSet<Entry> Entries { get; set; } = new HashSet<Entry>();
        public string LastEmail { get; set; }
    }
}
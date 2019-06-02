using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AliceInventory.Data
{
    public class UserData
    {
        public HashSet<Data.Entry> Entries { get; set; } = new HashSet<Entry>();
        public string LastEmail { get; set; }
    }
}

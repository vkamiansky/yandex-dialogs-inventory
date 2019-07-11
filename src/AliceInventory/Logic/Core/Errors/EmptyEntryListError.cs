using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AliceInventory.Logic.Core.Errors
{
    public class EmptyEntryListError : Error
    {
        public EmptyEntryListError() : base("Entries list is empty") { }
    }
}

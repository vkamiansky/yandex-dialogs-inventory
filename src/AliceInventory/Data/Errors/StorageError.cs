using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AliceInventory.Logic;

namespace AliceInventory.Data.Errors
{
    public class StorageError : Error
    {
        public string DatabaseName { get; set; }
        public string UserId { get; set; }

        public StorageError(string message) : base(message)
        {

        }
    }
}

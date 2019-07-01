using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AliceInventory.Logic
{
    public class Error
    {
        public string Message { get; }

        public Error()
        { }
        public Error(string message)
        { Message = message; }
    }
}

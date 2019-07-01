using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AliceInventory.Logic
{
    public class Error : OperationResult
    {
        public string Message { get; set; }

        public Error()
        { }
        public Error(string message)
        { Message = message; }
    }
}

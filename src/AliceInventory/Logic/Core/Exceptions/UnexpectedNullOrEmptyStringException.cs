using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AliceInventory.Logic.Core.Exceptions
{
    public class UnexpectedNullOrEmptyStringException : Exception
    {
        public string VariableName { get; private set; }

        public UnexpectedNullOrEmptyStringException(string variableName)
            : base($"{variableName} is null or empty")
        {
            VariableName = variableName;
        }

    }
}

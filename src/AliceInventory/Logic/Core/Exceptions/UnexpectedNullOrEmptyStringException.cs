using System;

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

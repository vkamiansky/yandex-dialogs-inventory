using System;

namespace AliceInventory.Logic.Core.Exceptions
{
    public class UnexpectedNullOrEmptyStringException : Exception
    {
        public UnexpectedNullOrEmptyStringException(string variableName)
            : base($"{variableName} is null or empty")
        {
            VariableName = variableName;
        }

        public string VariableName { get; }
    }
}
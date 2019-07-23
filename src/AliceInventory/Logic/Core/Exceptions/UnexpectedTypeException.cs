using System;

namespace AliceInventory.Logic.Core.Exceptions
{
    public class UnexpectedTypeException : Exception
    {
        public Type ExpectedType { get; private set; }
        public object ActualObject { get; private set; }

        public UnexpectedTypeException(object actualObject, Type expectedType)
            : base($"Expected {expectedType}, but actual {actualObject.GetType()}")
        {
            ActualObject = actualObject;
        }
    }
}

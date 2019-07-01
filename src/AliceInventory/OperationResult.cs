using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AliceInventory.Logic;

namespace AliceInventory
{
    public class OperationResult
    {
        public static OperationResult Ok = new OperationResult();

        public static implicit operator OperationResult(Error error)
        {
            return new OperationResult(error);
        }

        public bool HasError { get; }
        public Error Error { get; }

        public OperationResult() { }

        public OperationResult(Error error)
        {
            Error = error;
            HasError = true;
        }
    }

    public class OperationResult<T> : OperationResult
    {
        public static implicit operator OperationResult<T>(T obj)
        {
            return new OperationResult<T>(obj);
        }

        public T Object { get; }

        public OperationResult(T obj)
        {
            Object = obj;
        }

        public OperationResult(Error error) : base(error) { }
    }
}

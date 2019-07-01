using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AliceInventory.Logic
{
    public class OperationResult
    {
        public static OperationResult Ok = new OperationResult();
    }

    public class OperationResult<T>
    {
        public static implicit operator OperationResult<T>(Error error)
        {
            return new OperationResult<T>(error);
        }

        public static implicit operator OperationResult<T>(T obj)
        {
            return new OperationResult<T>(obj);
        }

        public bool IsError { get; }
        public T Object { get; }
        public Error Error { get; }

        public OperationResult(T obj)
        {
            Object = obj;
        }

        public OperationResult(Error error)
        {
            Error = error;
            IsError = true;
        }
    }
}

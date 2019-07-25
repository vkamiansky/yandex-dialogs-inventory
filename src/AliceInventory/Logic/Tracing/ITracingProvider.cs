using System;
using System.Threading.Tasks;
using OpenTracing;

namespace AliceInventory.Logic.Tracing
{
    public interface ITracingProvider
    {
        T TryTrace<T>(string operationName, Func<ISpan, T> operation);
        Task<T> TryTraceAsync<T>(string operationName, Func<ISpan, Task<T>> operation);
    }
}
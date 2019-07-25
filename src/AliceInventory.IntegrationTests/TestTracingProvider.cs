using System;
using System.Threading.Tasks;
using AliceInventory.Logic.Tracing;
using OpenTracing;

namespace AliceInventory.IntegrationTests
{
    public class TestTracingProvider : ITracingProvider
    {
        public T TryTrace<T>(string operationName, Func<ISpan, T> operation)
        {
            return operation(null);
        }

        public async Task<T> TryTraceAsync<T>(string operationName, Func<ISpan, Task<T>> operation)
        {
            return await operation(null);
        }
    }
}
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenTracing;

namespace AliceInventory.Logic.Tracing
{
    public class JaegerTracingProvider : ITracingProvider
    {
        private readonly ITracer _tracer;

        public JaegerTracingProvider(IConfigurationService configuration)
        {
            try
            {
                Environment.SetEnvironmentVariable("JAEGER_SERVICE_NAME", "alice-inventory");
                Environment.SetEnvironmentVariable("JAEGER_AGENT_HOST", configuration.GetTracingHost().Result);
                Environment.SetEnvironmentVariable("JAEGER_AGENT_PORT", configuration.GetTracingPort().Result.ToString());
                Environment.SetEnvironmentVariable("JAEGER_SAMPLER_TYPE", "const");

                var loggerFactory = new LoggerFactory();

                var config = Jaeger.Configuration.FromEnv(loggerFactory);
                _tracer = config.GetTracer();
            }
            catch
            {
                //Tracing initialization failed for some reason. No tracing mode is on.
            }
        }

        public T TryTrace<T>(string operationName, Func<ISpan, T> operation)
        {
            if (operation == null) return default(T);
            if (_tracer != null) // Tracing mode
            {
                var builder = _tracer.BuildSpan(operationName);
                using (var scope = builder.StartActive(true))
                {
                    var span = scope.Span;
                    try
                    {
                        return operation(span);
                    }
                    catch(Exception ex)
                    {
                        span.Log($"Unhandled exception while executing {operationName}:{ex.Message}");
                        return default(T);
                    }
                }
            }
            else // No tracing mode
            {
                try
                {
                    return operation(null);
                }
                catch
                {
                    return default(T);
                }
            }
        }

        public async Task<T> TryTraceAsync<T>(string operationName, Func<ISpan, Task<T>> operation)
        {
            if (operation == null) return default(T);
            if (_tracer != null) // Tracing mode
            {
                var builder = _tracer.BuildSpan(operationName);
                using (var scope = builder.StartActive(true))
                {
                    var span = scope.Span;
                    try
                    {
                        return await operation(span);
                    }
                    catch(Exception ex)
                    {
                        span.Log($"Unhandled exception while executing {operationName}:{ex.Message}");
                        return default(T);
                    }
                }
            }
            else // No tracing mode
            {
                try
                {
                    return await operation(null);
                }
                catch
                {
                    return default(T);
                }
            }
        }
    }
}
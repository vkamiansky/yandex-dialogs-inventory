using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OpenTracing;
using OpenTracing.Util;
using Jaeger;
using Jaeger.Reporters;
using Jaeger.Samplers;

namespace AliceInventory.Logger
{
    public class JaegerTracingOptions
    {
        public double SamplingRate { get; set; }
        public double LowerBound { get; set; }
        public ILoggerFactory LoggerFactory { get; set; }
        public string JaegerAgentHost{ get; set; }
        public int JaegerAgentPort { get; set; }
        public string ServiceName { get; set; }

        public JaegerTracingOptions()
        {
            SamplingRate = 0.1d;
            LowerBound = 1d;
            LoggerFactory = new LoggerFactory();
            JaegerAgentHost= "localhost";
            JaegerAgentPort = 16686;
            ServiceName = "jaeger-service";
        }
    }

    public static class JaegerTracingServiceCollectionExtensions
    {
        public static void AddJaegerTracing_Working(this IServiceCollection services)
        {
            services.AddOpenTracing();
            services.AddSingleton<ITracer>(serviceProvider =>
            {
                string serviceName = serviceProvider.GetRequiredService<IHostingEnvironment>().ApplicationName;

                // This will log to a default localhost installation of Jaeger.
                var tracer = new Tracer.Builder(serviceName)
                    .WithSampler(new ConstSampler(true))
                    .Build();

                // Allows code that can't use DI to also access the tracer.
                GlobalTracer.Register(tracer);

                return tracer;
            });
        }

         public static void ConfigureJaegerTracing(this IServiceCollection services,
            Action<JaegerTracingOptions> setupAction)
        {
            services.Configure<JaegerTracingOptions>(setupAction);
        }

        public static void AddJaegerTracing_NotWorking(this IServiceCollection services,
            Action<JaegerTracingOptions> setupAction = null)
        {
            if (setupAction != null) 
                services.ConfigureJaegerTracing(setupAction);

            services.AddSingleton<ITracer>(cli =>
            {
                var options = cli.GetService<IOptions<JaegerTracingOptions>>().Value;

                var senderConfig = new Jaeger.Configuration.SenderConfiguration(options.LoggerFactory)
                    .WithAgentHost(options.JaegerAgentHost)
                    .WithAgentPort(options.JaegerAgentPort);

                var reporter = new RemoteReporter.Builder()
                    .WithLoggerFactory(options.LoggerFactory)
                    .WithSender(senderConfig.GetSender())
                    .Build();

                var sampler = new GuaranteedThroughputSampler(options.SamplingRate, options.LowerBound);
                
                var tracer = new Tracer.Builder(options.ServiceName)
                    .WithLoggerFactory(options.LoggerFactory)
                    .WithReporter(reporter)
                    .WithSampler(sampler)
                    .Build();

                // Allows code that can't use dependency injection to have access to the tracer.
                if (!GlobalTracer.IsRegistered())
                    GlobalTracer.Register(tracer);

                return tracer;
            });

            services.AddOpenTracing(builder => {
                builder.ConfigureAspNetCore(options => {
                    options.Hosting.IgnorePatterns.Add(x => {
                        return x.Request.Path == "/health";
                    });
                    options.Hosting.IgnorePatterns.Add(x => {
                        return x.Request.Path == "/metrics";
                    });
                });
            });
        }
    }
}

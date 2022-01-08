using _4oito6.Demonstration.AuditTrail.IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace _4oito6.Demonstration.AuditTrail.Receiver
{
    public static class IoC
    {
        public static ServiceProvider Provider { get; private set; }

        static IoC()
        {
            IServiceCollection services = new ServiceCollection();

            // adding configuration
            services.AddScoped<IConfiguration>
            (
                sp => new ConfigurationBuilder()
                    .SetBasePath
                    (
                        basePath: Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").Equals("Local") ?
                            Directory.GetCurrentDirectory().Replace("bin\\Debug\\netcoreapp3.1", string.Empty) :
                            Directory.GetCurrentDirectory()
                    )
                    .AddJsonFile(path: "appsettings.json", optional: true)
                    .Build()
            );

            // adding aws dependencies
            services.AddAwsDependencies();

            // adding log:
            services.AddLogging();
            services.AddScoped<ILoggerFactory, LoggerFactory>();
            services.AddScoped(typeof(ILogger<>), typeof(Logger<>));

            // add receiver dependencies:
            services.AddReceiverDependencies();

            Provider = services.BuildServiceProvider();
        }
    }
}
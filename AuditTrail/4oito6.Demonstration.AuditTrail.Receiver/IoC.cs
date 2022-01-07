using _4oito6.Demonstration.AuditTrail.IoC;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace _4oito6.Demonstration.AuditTrail.Receiver
{
    public static class IoC
    {
        public static ServiceProvider Provider { get; private set; }

        static IoC()
        {
            IServiceCollection services = new ServiceCollection();

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
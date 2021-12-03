using _4oito6.Demonstration.Contact.Domain.EntryPoint;
using _4oito6.Demonstration.Contact.Domain.EntryPoint.IoC;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddAwsDependencies();
        services.AddContact();
    })
    .Build();

await host.RunAsync();
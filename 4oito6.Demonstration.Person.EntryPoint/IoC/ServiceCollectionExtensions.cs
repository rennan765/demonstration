using _4oito6.Demonstration.Config;
using _4oito6.Demonstration.Data.Connection;
using _4oito6.Demonstration.Person.Application;
using _4oito6.Demonstration.Person.Application.Interfaces;
using _4oito6.Demonstration.Person.Data.Transaction;
using _4oito6.Demonstration.Person.Domain.Data.Repositories;
using _4oito6.Demonstration.Person.Domain.Data.Transaction;
using _4oito6.Demonstration.Person.Domain.Services;
using _4oito6.Demonstration.Person.Domain.Services.Interfaces;
using _4oito6.Demonstration.Person.EntryPoint.IoC.Config;
using Npgsql;

namespace _4oito6.Demonstration.Person.EntryPoint.IoC
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPersonApi(this IServiceCollection services)
        {
            // add dependencies:
            services.AddSingleton<IPersonConfig, PersonConfig>().
                AddSingleton<ICommonConfig>(sp => sp.GetService<IPersonConfig>());

            // add data:
            services
                .AddScoped<IPersonUnitOfWork>
                (
                    sp =>
                    {
                        var config = sp.GetService<IPersonConfig>() ?? throw new ArgumentNullException(nameof(IPersonConfig));
                        return new PersonUnitOfWork
                        (
                            relationalDatabase: new AsyncDbConnection
                            (
                                conn: new NpgsqlConnection(config.GetRelationalDatabaseConnectionString().Result)
                            )
                        );
                    }
                )
                .AddScoped<IPersonRepositoryRoot>(sp => sp.GetService<IPersonUnitOfWork>());

            //add service layer:
            services.AddScoped<IPersonServices, PersonServices>();

            // add application layer:
            services.AddScoped<IPersonAppServices, PersonAppServices>();

            return services;
        }
    }
}
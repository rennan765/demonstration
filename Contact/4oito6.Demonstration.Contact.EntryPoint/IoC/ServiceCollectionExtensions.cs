using _4oito6.Demonstration.Config;
using _4oito6.Demonstration.Contact.Application;
using _4oito6.Demonstration.Contact.Application.Interfaces;
using _4oito6.Demonstration.Contact.Data.Transaction;
using _4oito6.Demonstration.Contact.Domain.Data.Repositories;
using _4oito6.Demonstration.Contact.Domain.Data.Transaction;
using _4oito6.Demonstration.Contact.Domain.Services;
using _4oito6.Demonstration.Contact.Domain.Services.Interfaces;
using _4oito6.Demonstration.Contact.EntryPoint.IoC.Config;
using _4oito6.Demonstration.CrossCutting.AuditTrail;
using _4oito6.Demonstration.CrossCutting.AuditTrail.Interface;
using _4oito6.Demonstration.Data.Connection;
using _4oito6.Demonstration.Data.Connection.MySql;
using _4oito6.Demonstration.SQS;
using Amazon;
using Amazon.SecretsManager;
using Amazon.SQS;
using MySqlConnector;
using Npgsql;
using System.Diagnostics.CodeAnalysis;

namespace _4oito6.Demonstration.Contact.EntryPoint.IoC
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAwsDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IAmazonSecretsManager>
            (
                sp => new AmazonSecretsManagerClient
                (
                    awsAccessKeyId: Environment.GetEnvironmentVariable("AwsAccessKeyId"),
                    awsSecretAccessKey: Environment.GetEnvironmentVariable("AwsSecretKey"),
                    region: RegionEndpoint.GetBySystemName(Environment.GetEnvironmentVariable("AwsRegion"))
                )
            );

            services.AddScoped<IAmazonSQS>
            (
                sp => new AmazonSQSClient
                (
                    awsAccessKeyId: Environment.GetEnvironmentVariable("AwsAccessKeyId"),
                    awsSecretAccessKey: Environment.GetEnvironmentVariable("AwsSecretKey"),
                    region: RegionEndpoint.GetBySystemName(Environment.GetEnvironmentVariable("AwsRegion"))
                )
            );

            return services;
        }

        public static IServiceCollection AddContact(this IServiceCollection services)
        {
            // add dependencies:
            services.AddSingleton<IContactConfig, ContactConfig>().
                AddSingleton<ICommonConfig>(sp => sp.GetService<IContactConfig>());

            // add helpers:
            services
                .AddScoped<IAuditTrailSender>
                (
                    sp => new AuditTrailSender
                    (
                        sqs: sp.GetService<IAmazonSQS>(),
                        queue: sp.GetService<IContactConfig>().AuditTrailQueueUrl
                    )
                );

            // add data:
            services
                .AddScoped<IContactUnitOfWork>
                (
                    sp =>
                    {
                        var config = sp.GetService<IContactConfig>() ?? throw new ArgumentNullException(nameof(IContactConfig));
                        return new ContactUnitOfWork
                        (
                            relationalDatabase: new AsyncDbConnection
                            (
                                connection: new NpgsqlConnection(config.GetRelationalDatabaseConnectionString().Result)
                            ),

                            cloneDatabase: new MySqlAsyncDbConnection
                            (
                                connection: new MySqlConnection(config.GetCloneDatabaseConnectionString().Result)
                            )
                        );
                    }
                )
                .AddScoped<IContactRepositoryRoot>(sp => sp.GetService<IContactUnitOfWork>());

            //add service layer:
            services
                .AddScoped<IContactServices, ContactServices>()
                .AddScoped<ICloningServices, CloningServices>();

            // add application layer:
            services.AddScoped<IContactAppServices>
            (
                sp =>
                {
                    var config = sp.GetService<IContactConfig>();
                    return new ContactAppServices
                    (
                        contact: sp.GetService<IContactServices>(),
                        cloning: sp.GetService<ICloningServices>(),

                        uow: sp.GetService<IContactUnitOfWork>(),
                        sqs: new SQSHelper
                        (
                            sp.GetService<IAmazonSQS>(),
                            queue: config.ContactQueueUrl
                        ),

                        logger: sp.GetService<ILogger<ContactAppServices>>(),
                        auditTrail: sp.GetService<IAuditTrailSender>()
                    );
                }
            );

            return services;
        }
    }
}
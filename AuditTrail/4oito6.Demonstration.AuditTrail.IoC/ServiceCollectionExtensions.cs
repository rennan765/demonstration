using _4oito6.Demonstration.AuditTrail.IoC.Config;
using _4oito6.Demonstration.AuditTrail.Receiver.Application;
using _4oito6.Demonstration.AuditTrail.Receiver.Data;
using _4oito6.Demonstration.AuditTrail.Receiver.Domain.Data;
using _4oito6.Demonstration.AuditTrail.Receiver.Domain.Services;
using _4oito6.Demonstration.Config;
using _4oito6.Demonstration.CrossCutting.AuditTrail;
using _4oito6.Demonstration.CrossCutting.AuditTrail.Interface;
using _4oito6.Demonstration.Data.Connection;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.SecretsManager;
using Amazon.SQS;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace _4oito6.Demonstration.AuditTrail.IoC
{
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

            services.AddScoped<IAmazonDynamoDB>
            (
                sp => new AmazonDynamoDBClient
                (
                    awsAccessKeyId: Environment.GetEnvironmentVariable("AwsAccessKeyId"),
                    awsSecretAccessKey: Environment.GetEnvironmentVariable("AwsSecretKey"),
                    region: RegionEndpoint.GetBySystemName(Environment.GetEnvironmentVariable("AwsRegion"))
                )
            );

            return services;
        }

        public static IServiceCollection AddReceiverDependencies(this IServiceCollection services)
        {
            // adding config:
            services.AddScoped<IAuditTrailConfig, AuditTrailConfig>();
            services.AddScoped<ICommonConfig>(sp => sp.GetService<IAuditTrailConfig>());

            // adding other dependencies:
            services.AddScoped<IAuditTrailSender>
                (
                    sp => new AuditTrailSender
                    (
                        sqs: sp.GetService<IAmazonSQS>(),
                        queue: sp.GetService<ICommonConfig>().AuditTrailQueueUrl
                    )
                );

            // adding data:
            services.AddScoped(typeof(IDynamoConnection<>), typeof(DynamoConnection<>));

            // adding context services
            services
                .AddScoped<IAuditTrailRepository, AuditTrailRepository>()
                .AddScoped<IAuditTrailServices, AuditTrailServices>()
                .AddScoped<IAuditTrailAppServices, AuditTrailAppServices>();

            return services;
        }
    }
}
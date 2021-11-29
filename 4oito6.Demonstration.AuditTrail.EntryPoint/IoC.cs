using _4oito6.Demonstration.AuditTrail.EntryPoint.Application;
using _4oito6.Demonstration.AuditTrail.EntryPoint.Data;
using _4oito6.Demonstration.AuditTrail.EntryPoint.Domain.Data;
using _4oito6.Demonstration.AuditTrail.EntryPoint.Domain.Services;
using _4oito6.Demonstration.Config;
using _4oito6.Demonstration.CrossCutting.AuditTrail;
using _4oito6.Demonstration.CrossCutting.AuditTrail.Interface;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.SecretsManager;
using Amazon.SQS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace _4oito6.Demonstration.AuditTrail.EntryPoint
{
    public static class IoC
    {
        public static ServiceProvider Provider { get; private set; }

        static IoC()
        {
            IServiceCollection services = new ServiceCollection();

            // adding aws dependencies
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

            // adding log:
            services.AddLogging();
            services.AddScoped<ILoggerFactory, LoggerFactory>();
            services.AddScoped(typeof(ILogger<>), typeof(Logger<>));

            // adding other dependencies:
            services.AddScoped<ICommonConfig, CommonConfig>()
                .AddScoped<IAuditTrailSender>
                (
                    sp => new AuditTrailSender
                    (
                        sqs: sp.GetService<IAmazonSQS>(),
                        queue: sp.GetService<ICommonConfig>().AuditTrailQueueUrl
                    )
                );

            // adding context services
            services
                .AddScoped<IAuditTrailRepository, AuditTrailRepository>()
                .AddScoped<IAuditTrailServices, AuditTrailServices>()
                .AddScoped<IAuditTrailAppServices, AuditTrailAppServices>();

            Provider = services.BuildServiceProvider();
        }
    }
}
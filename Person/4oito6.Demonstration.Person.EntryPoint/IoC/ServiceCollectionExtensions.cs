using _4oito6.Demonstration.Config;
using _4oito6.Demonstration.CrossCutting.AuditTrail;
using _4oito6.Demonstration.CrossCutting.AuditTrail.Interface;
using _4oito6.Demonstration.Data.Connection;
using _4oito6.Demonstration.Person.Application;
using _4oito6.Demonstration.Person.Application.Interfaces;
using _4oito6.Demonstration.Person.Data.Transaction;
using _4oito6.Demonstration.Person.Domain.Data.Repositories;
using _4oito6.Demonstration.Person.Domain.Data.Transaction;
using _4oito6.Demonstration.Person.Domain.Services;
using _4oito6.Demonstration.Person.Domain.Services.Interfaces;
using _4oito6.Demonstration.Person.EntryPoint.Filters;
using _4oito6.Demonstration.Person.EntryPoint.IoC.Config;
using Amazon;
using Amazon.SecretsManager;
using Amazon.SQS;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;
using Npgsql;

namespace _4oito6.Demonstration.Person.EntryPoint.IoC
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

            return services;
        }

        public static IServiceCollection AddPersonApi(this IServiceCollection services)
        {
            // add dependencies:
            services.AddSingleton<IPersonConfig, PersonConfig>().
                AddSingleton<ICommonConfig>(sp => sp.GetService<IPersonConfig>());

            // add helpers:
            services
                .AddScoped<IAuditTrailSender>
                (
                    sp => new AuditTrailSender
                    (
                        sqs: sp.GetService<IAmazonSQS>(),
                        queue: sp.GetService<IPersonConfig>().AuditTrailQueueUrl
                    )
                );

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

        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            var config = services.BuildServiceProvider().GetService<IPersonConfig>()?.SwaggerConfig;

            if (config is null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            services.AddSwaggerGen(x =>
            {
                //Documentation data
                var info = new OpenApiInfo
                {
                    Title = config.Title ?? throw new ArgumentNullException(nameof(config.Title)),
                    Version = config.Version ?? throw new ArgumentNullException(nameof(config.Version)),
                    Description = config.Description ?? throw new ArgumentNullException(nameof(config.Description))
                };

                if (!string.IsNullOrEmpty(config.ContactName))
                    info.Contact = new OpenApiContact
                    {
                        Name = config.ContactName,
                        Email = config.ContactEmail,
                        Url = new Uri(config.ContactUrl)
                    };

                x.SwaggerDoc("v1", info);

                //Insert token
                x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Json Web Token (JWT) Authorization header using the Bearer scheme."
                });

                x.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                    }
                });

                //Insert refresh token's parameter header
                x.OperationFilter<RefreshTokenFilter>();

                //Insert comment's XML
                var xmlDocumentPath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, $"{PlatformServices.Default.Application.ApplicationName}.xml");

                if (File.Exists(xmlDocumentPath))
                {
                    x.IncludeXmlComments(xmlDocumentPath);
                }
            });

            services.AddSwaggerGenNewtonsoftSupport();

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
        {
            var config = services.BuildServiceProvider().GetService<IPersonConfig>().TokenConfig;
            if (config is null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            services.AddAuthentication
            (
                authOptions =>
                {
                    authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }
            )
            .AddJwtBearer
            (
                bearerOptions =>
                {
                    var paramsValidation = bearerOptions.TokenValidationParameters;

                    paramsValidation.IssuerSigningKey = config.SigningCredentials.Key;
                    paramsValidation.ValidAudience = config.Audience;
                    paramsValidation.ValidIssuer = config.Issuer;

                    // Valida a assinatura de um token recebido
                    paramsValidation.ValidateIssuerSigningKey = true;

                    // Verifica se um token recebido ainda é válido
                    paramsValidation.ValidateLifetime = true;

                    // Tempo de tolerância para a expiração de um token (utilizado caso haja problemas de sincronismo
                    //de horário entre diferentes computadores envolvidos no processo de comunicação)
                    paramsValidation.ClockSkew = TimeSpan.Zero;
                }
            );

            services.AddAuthorization
            (
                auth =>
                {
                    auth.AddPolicy
                    (
                        "Bearer",
                        new AuthorizationPolicyBuilder()
                            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                            .RequireAuthenticatedUser().Build()
                    );
                }
            );

            return services;
        }
    }
}
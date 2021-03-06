using _4oito6.Demonstration.AuditTrail.IoC;
using _4oito6.Demonstration.Config;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;
using System.Diagnostics.CodeAnalysis;

namespace _4oito6.Demonstration.AuditTrail.Sender.IoC
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAuditTrailSender(this IServiceCollection services)
        {
            // add aws dependencies
            services.AddAwsDependencies();

            // add receiver dependencies:
            services.AddReceiverDependencies();

            // add swagger:
            var config = services.BuildServiceProvider().GetService<ICommonConfig>()?.SwaggerConfig;

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
    }
}
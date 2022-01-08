using _4oito6.Demonstration.Config;
using Amazon.SecretsManager;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics.CodeAnalysis;

namespace _4oito6.Demonstration.AuditTrail.IoC.Config
{
    [ExcludeFromCodeCoverage]
    public class AuditTrailConfig : CommonConfig, IAuditTrailConfig
    {
        public AuditTrailConfig(IConfiguration configuration, IAmazonSecretsManager secretsManager)
            : base(configuration, secretsManager)
        {
        }

        public override string AuditTrailQueueUrl
        {
            get
            {
                var content = Environment.GetEnvironmentVariable("AuditTrailQueueUrl");
                if (!string.IsNullOrEmpty(content))
                {
                    return content;
                }

                return base.AuditTrailQueueUrl;
            }
        }
    }
}
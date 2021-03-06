using _4oito6.Demonstration.Config;
using Amazon.SecretsManager;
using System.Diagnostics.CodeAnalysis;

namespace _4oito6.Demonstration.Contact.EntryPoint.IoC.Config
{
    [ExcludeFromCodeCoverage]
    public class ContactConfig : CommonConfig, IContactConfig
    {
        public ContactConfig(IConfiguration configuration, IAmazonSecretsManager secretsManager)
            : base(configuration, secretsManager)
        {
        }

        public string ContactQueueUrl => Environment.GetEnvironmentVariable("ContactQueueUrl");
    }
}
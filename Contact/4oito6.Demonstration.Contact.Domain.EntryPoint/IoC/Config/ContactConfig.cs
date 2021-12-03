using _4oito6.Demonstration.Config;
using Amazon.SecretsManager;

namespace _4oito6.Demonstration.Contact.Domain.EntryPoint.IoC.Config
{
    public class ContactConfig : CommonConfig, IContactConfig
    {
        public ContactConfig(IConfiguration configuration, IAmazonSecretsManager secretsManager)
            : base(configuration, secretsManager)
        {
        }

        public string ContactQueueUrl => Environment.GetEnvironmentVariable("ContactQueueUrl");
    }
}
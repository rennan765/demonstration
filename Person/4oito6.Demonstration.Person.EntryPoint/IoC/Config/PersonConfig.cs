using _4oito6.Demonstration.Config;
using Amazon.SecretsManager;
using System.Diagnostics.CodeAnalysis;

namespace _4oito6.Demonstration.Person.EntryPoint.IoC.Config
{
    [ExcludeFromCodeCoverage]
    public class PersonConfig : CommonConfig, IPersonConfig
    {
        public PersonConfig(IConfiguration configuration, IAmazonSecretsManager secretsManager)
            : base(configuration, secretsManager)
        {
        }

        public string PersonQueueUrl => Environment.GetEnvironmentVariable("PersonQueueUrl");
    }
}
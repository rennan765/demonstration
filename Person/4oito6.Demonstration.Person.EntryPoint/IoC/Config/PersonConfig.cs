using _4oito6.Demonstration.Config;
using Amazon.SecretsManager;

namespace _4oito6.Demonstration.Person.EntryPoint.IoC.Config
{
    public class PersonConfig : CommonConfig, IPersonConfig
    {
        public PersonConfig(IConfiguration configuration, IAmazonSecretsManager secretsManager)
            : base(configuration, secretsManager)
        {
        }
    }
}
using _4oito6.Demonstration.Config;

namespace _4oito6.Demonstration.Person.EntryPoint.IoC.Config
{
    public class PersonConfig : CommonConfig, IPersonConfig
    {
        public PersonConfig(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
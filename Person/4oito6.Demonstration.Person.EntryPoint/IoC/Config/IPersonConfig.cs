using _4oito6.Demonstration.Config;

namespace _4oito6.Demonstration.Person.EntryPoint.IoC.Config
{
    public interface IPersonConfig : ICommonConfig
    {
        string PersonQueueUrl { get; }
    }
}
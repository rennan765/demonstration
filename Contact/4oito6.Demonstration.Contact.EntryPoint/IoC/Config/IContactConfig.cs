using _4oito6.Demonstration.Config;

namespace _4oito6.Demonstration.Contact.EntryPoint.IoC.Config
{
    public interface IContactConfig : ICommonConfig
    {
        string ContactQueueUrl { get; }
    }
}
using _4oito6.Demonstration.Config;

namespace _4oito6.Demonstration.Contact.Domain.EntryPoint.IoC.Config
{
    public interface IContactConfig : ICommonConfig
    {
        string ContactQueueUrl { get; }
    }
}
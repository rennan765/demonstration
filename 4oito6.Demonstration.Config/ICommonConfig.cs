using System.Threading.Tasks;

namespace _4oito6.Demonstration.Config
{
    public interface ICommonConfig
    {
        bool IsLocal { get; }

        string AwsRegion { get; }

        Task<string> GetRelationalDatabaseConnectionString();
    }
}
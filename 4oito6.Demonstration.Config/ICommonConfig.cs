using _4oito6.Demonstration.Config.Model;
using System;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.Config
{
    public interface ICommonConfig : IDisposable
    {
        bool IsLocal { get; }

        string AwsAccessKeyId { get; }

        string AwsSecretKey { get; }

        string AwsRegion { get; }

        SwaggerConfig SwaggerConfig { get; }

        TokenConfig TokenConfig { get; }

        string AuditTrailQueueUrl { get; }

        Task<string> GetRelationalDatabaseConnectionString();
    }
}
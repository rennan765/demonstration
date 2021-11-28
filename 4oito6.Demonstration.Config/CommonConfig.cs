using System;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.Config
{
    public class CommonConfig : ICommonConfig
    {
        public string AwsRegion => Environment.GetEnvironmentVariable("AwsRegion");

        public bool IsLocal => Environment.GetEnvironmentVariable("Environment").Equals("Local");

        public Task<string> GetRelationalDatabaseConnectionString()
        {
            return Task.FromResult(Environment.GetEnvironmentVariable("RelationalDatabaseConnectionString"));
        }
    }
}
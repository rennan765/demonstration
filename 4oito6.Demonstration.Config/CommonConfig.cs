using _4oito6.Demonstration.Config.Model;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.Config
{
    public class CommonConfig : ICommonConfig
    {
        private readonly IConfiguration _configuration;
        private SwaggerConfig _swaggerConfig;

        public CommonConfig(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public string AwsRegion => Environment.GetEnvironmentVariable("AwsRegion");

        public bool IsLocal => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").Equals("Local");

        public SwaggerConfig SwaggerConfig
        {
            get
            {
                if (_swaggerConfig is null)
                {
                    _swaggerConfig = JsonConvert
                        .DeserializeObject<SwaggerConfig>(_configuration.GetSection("SwaggerConfig").Value);
                }

                return _swaggerConfig;
            }
        }

        public Task<string> GetRelationalDatabaseConnectionString()
        {
            return Task.FromResult(Environment.GetEnvironmentVariable("RelationalDatabaseConnectionString"));
        }
    }
}
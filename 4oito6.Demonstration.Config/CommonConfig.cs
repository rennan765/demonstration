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
        private TokenConfig _tokenConfig;

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

        public TokenConfig TokenConfig
        {
            get
            {
                if (_tokenConfig is null)
                {
                    _tokenConfig = new TokenConfig
                    (
                        issuer: Environment.GetEnvironmentVariable("Issuer"),
                        audience: Environment.GetEnvironmentVariable("Audience"),

                        secretKey: Environment.GetEnvironmentVariable("SecretKey"),
                        tokenTime: Convert.ToInt32(Environment.GetEnvironmentVariable("TokenTime"))
                    );
                }

                return _tokenConfig;
            }
        }

        public Task<string> GetRelationalDatabaseConnectionString()
        {
            return Task.FromResult(Environment.GetEnvironmentVariable("RelationalDatabaseConnectionString"));
        }
    }
}
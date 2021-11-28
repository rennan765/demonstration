using _4oito6.Demonstration.Commons;
using _4oito6.Demonstration.Config.Model;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.Config
{
    public class CommonConfig : DisposableObject, ICommonConfig
    {
        private readonly IAmazonSecretsManager _secretsManager;
        private readonly IConfiguration _configuration;
        private SwaggerConfig _swaggerConfig;
        private TokenConfig _tokenConfig;

        public CommonConfig(IConfiguration configuration, IAmazonSecretsManager secretsManager)
            : base(new IDisposable[] { secretsManager })
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _secretsManager = secretsManager ?? throw new ArgumentNullException(nameof(secretsManager));
        }

        public string AwsAccessKeyId => Environment.GetEnvironmentVariable("AwsAccessKeyId");

        public string AwsSecretKey => Environment.GetEnvironmentVariable("AwsSecretKey");

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
                    if (IsLocal)
                    {
                        _tokenConfig = new TokenConfig
                        (
                            issuer: Environment.GetEnvironmentVariable("Issuer"),
                            audience: Environment.GetEnvironmentVariable("Audience"),

                            secretKey: Environment.GetEnvironmentVariable("SecretKey"),
                            tokenTime: Convert.ToInt32(Environment.GetEnvironmentVariable("TokenTime"))
                        );
                    }
                    else
                    {
                        _tokenConfig = JsonConvert.DeserializeObject<TokenConfig>(GetSecretString("TokenConfig").Result);
                    }
                }

                return _tokenConfig;
            }
        }

        protected async Task<string> GetSecretString(string secretName)
        {
            var request = new GetSecretValueRequest
            {
                SecretId = secretName,
                VersionStage = "AWSCURRENT" // VersionStage defaults to AWSCURRENT if unspecified.
            };

            var response = await _secretsManager.GetSecretValueAsync(request).ConfigureAwait(false);
            return response.SecretString;
        }

        public Task<string> GetRelationalDatabaseConnectionString()
        {
            if (!IsLocal)
            {
                return GetSecretString("RelationalDatabaseConnectionString");
            }

            return Task.FromResult(Environment.GetEnvironmentVariable("RelationalDatabaseConnectionString"));
        }
    }
}
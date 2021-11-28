using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace _4oito6.Demonstration.Config.Model
{
    public class TokenConfig
    {
        public TokenConfig(string issuer, string audience, string secretKey, int tokenTime)
        {
            Issuer = issuer;
            Audience = audience;
            SecretKey = secretKey;
            TokenTime = tokenTime;

            SigningCredentials = new SigningCredentials
            (
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey)),
                SecurityAlgorithms.HmacSha256
            );
        }

        public string Issuer { get; private set; }
        public string Audience { get; private set; }
        private string SecretKey { get; set; }
        public int TokenTime { get; private set; }
        public SigningCredentials SigningCredentials { get; private set; }
    }
}
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;


namespace MarketBackend.Domain.Security
{
    public class TokenManager : ITokenManager
    {
        private static TokenManager tokenManagerInstance;
        private string _secretKey;
        private JwtSecurityTokenHandler tokenHandler;
        public int ExpirationTime {  get; set; }
        private TokenManager()
        {
            byte[] key = new byte[32]; // 256 bits
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(key);
            }
            _secretKey = Convert.ToBase64String(key);
            ExpirationTime = 24 * 60;
            tokenHandler = new JwtSecurityTokenHandler();
        }

        public static TokenManager GetInstance()
        {
            if (tokenManagerInstance == null)
            {
                tokenManagerInstance = new TokenManager();
            }
            return tokenManagerInstance;
        }
        public string GenerateToken(string username)
        {

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var currentTimeUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds(); // Get current time as Unix timestamp

            var token = new JwtSecurityToken(
                claims: new[] { new Claim("username", username),
                               new Claim(JwtRegisteredClaimNames.Iat, currentTimeUnix.ToString(), ClaimValueTypes.Integer64) },
                expires: DateTime.Now.AddMinutes(ExpirationTime),
                signingCredentials: credentials
            );

            return tokenHandler.WriteToken(token);
        }

        public bool ValidateToken(string token)
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey)),
                ClockSkew = TimeSpan.Zero // Set clock skew to zero for token expiration validation
            };

            try
            {
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return true;
            }
            catch (SecurityTokenException) // Token validation failed
            {
                //log
                return false;
            }
        }

        public string ExtractUsername(string token)
        {
           
            var jsonToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
            var usernameClaim = jsonToken?.Claims.FirstOrDefault(claim => claim.Type == "username");

            if (usernameClaim != null)
            {
                return usernameClaim.Value;
            }

            throw new SecurityTokenException("Invalid token or userId claim not found");
        }

        public DateTime ExtractIssuedAt(string token)
        {
            var jsonToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
            var issuedAtClaim = jsonToken?.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Iat);

            if (issuedAtClaim != null && long.TryParse(issuedAtClaim.Value, out long issuedAtUnix))
            {
                return DateTimeOffset.FromUnixTimeSeconds(issuedAtUnix).DateTime;
            }

            throw new SecurityTokenException("Invalid token or issued at claim not found");
        }

        public DateTime ExtractExpiration(string token)
        {
            var jsonToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            if (jsonToken != null && jsonToken.ValidTo != DateTime.MinValue)
            {
                return jsonToken.ValidTo;
            }

            throw new SecurityTokenException("Invalid token or expiration claim not found");
        }
    }
}

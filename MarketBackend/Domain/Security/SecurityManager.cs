using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketBackend.Services;
using Microsoft.AspNetCore.Identity;


namespace MarketBackend.Domain.Security
{
    public class SecurityManager : ISecurityManager, ITokenManager
    {
        PasswordHasher<object> passwordHasher;
        private ITokenManager tokenManager;
        private static SecurityManager securityManagerInstance;
        private SecurityManager() { 
            passwordHasher = new PasswordHasher<object>();
            tokenManager = TokenManager.GetInstance();
        }

        public static SecurityManager GetInstance()
        {
            if (securityManagerInstance == null)
            {
                securityManagerInstance = new SecurityManager();
            }
            return securityManagerInstance;
        }

        public string EncryptPassword(string password)
        {
            return passwordHasher.HashPassword(null, password);
        }

        public bool VerifyPassword(string rawPassword, string hashedPassword)
        {
            var result = passwordHasher.VerifyHashedPassword(null, hashedPassword, rawPassword);
            return result == PasswordVerificationResult.Success;
        }

        public string GenerateToken(string username)
        {
            return tokenManager.GenerateToken(username);
        }
        public bool ValidateToken(string token)
        {
            return tokenManager.ValidateToken(token);
        }

        public string ExtractUsername(string token)
        {
            return tokenManager.ExtractUsername(token);
        }

        public DateTime ExtractIssuedAt(string token)
        {
            return tokenManager.ExtractIssuedAt(token);
        }
        public DateTime ExtractExpiration(string token)
        {
            return tokenManager.ExtractExpiration(token);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketBackend.Domain.Security
{
    public interface ITokenManager
    {
        public string GenerateToken(string username);
        public bool ValidateToken(string token);
        public string ExtractUsername(string token);
        public DateTime ExtractIssuedAt(string token);
        public DateTime ExtractExpiration(string token);
    }
}

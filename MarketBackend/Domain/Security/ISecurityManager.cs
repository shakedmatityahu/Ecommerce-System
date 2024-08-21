using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketBackend.Domain.Security
{
    public interface ISecurityManager
    {
        string EncryptPassword(string password);
        bool VerifyPassword(string rawPassword, string hashedPassword);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketBackend.Domain.Market_Client;

namespace MarketBackend.Services.Interfaces
{
    public interface IClientRepository : IRepository<Member>
    {
        Member GetByUserName(string userName);
        bool ContainsUserName(string userName);
    }
}
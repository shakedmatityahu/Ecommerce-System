using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Text;

namespace MarketBackend.Domain.Market_Client
{
    public interface ISellMethod
    {
        void makeBid(int userId, double price);
        void approvedBid();
        string getSellMethod();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Text;

namespace MarketBackend.Domain.Market_Client
{
    public class BidSell : ISellMethod
    {   
        private ConcurrentDictionary<int, double> _potentialBuyers;
        private ConcurrentDictionary<int, double> _approvedBuyers;
        public BidSell()
        {
            _potentialBuyers = new ConcurrentDictionary<int, double>();
            _approvedBuyers = new ConcurrentDictionary<int, double>();
        }

        public void makeBid(int userId, double price)
        {
            
        }

        public void approvedBid()
        {
            
        }

        public string getSellMethod()
        {
            return "BidSell";
        }
    }
}
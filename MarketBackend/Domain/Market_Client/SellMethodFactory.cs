using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Text;

namespace MarketBackend.Domain.Market_Client
{
    public class SellMethodFactory
    {
        public static ISellMethod createSellMethod(string sellMethod)
        {
            switch (sellMethod)
            {
                case "RegularSell":
                    return new RegularSell();
                case "BidSell":
                    return new BidSell();
                case "AuctionSell":
                    return new AuctionSell();
                case "LotterySell":
                    return new LotterySell();    
                default:
                    return null;
            }
        }
    }
}
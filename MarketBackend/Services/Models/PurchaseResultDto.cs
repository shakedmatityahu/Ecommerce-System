using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketBackend.Domain.Market_Client;

namespace MarketBackend.Services.Models
{
    public class PurchaseResultDto
    {
        public double Price { get; set; }
    
        public PurchaseResultDto(Purchase purchase){
            Price = purchase.Price;        
        }    
    }
}
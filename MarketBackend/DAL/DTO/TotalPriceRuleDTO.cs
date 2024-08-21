using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketBackend.Domain.Market_Client;

namespace MarketBackend.DAL.DTO
{
    public class TotalPriceRuleDTO :RuleDTO
    {
        public double TotalPrice { get; set; }
        public TotalPriceRuleDTO() { }
        public TotalPriceRuleDTO(RuleSubjectDTO subject, double totalPrice) : base(subject)
        {
            TotalPrice = totalPrice;
        }
        public TotalPriceRuleDTO(TotalPriceRule rule) : base(rule) 
        {
            TotalPrice = rule.TotalPrice;
        }
    }
}
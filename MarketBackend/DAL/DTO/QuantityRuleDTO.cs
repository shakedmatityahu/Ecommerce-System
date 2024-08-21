using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketBackend.Domain.Market_Client;

namespace MarketBackend.DAL.DTO
{
    public class QuantityRuleDTO : RuleDTO
    {
        public int MinQuantity { get; set; }
        public int MaxQuantity { get; set; }
        public QuantityRuleDTO() { }
        public QuantityRuleDTO(RuleSubjectDTO subject, int minQuantity, int maxQuantity) : base(subject)
        {
            MinQuantity = minQuantity;
            MaxQuantity = maxQuantity;

        }
        public QuantityRuleDTO(QuantityRule rule) : base(rule)
        {
            MinQuantity = rule.MinQuantity;
            MaxQuantity = rule.MaxQuantity;
        }
    }
}
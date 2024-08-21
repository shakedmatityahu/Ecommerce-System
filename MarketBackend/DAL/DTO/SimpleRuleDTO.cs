using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketBackend.Domain.Market_Client;

namespace MarketBackend.DAL.DTO
{
    public class SimpleRuleDTO : RuleDTO
    {
        public SimpleRuleDTO() { }
        public SimpleRuleDTO(RuleSubjectDTO subject) : base(subject)
        {
        }
        public SimpleRuleDTO(SimpleRule rule) : base(rule){
        }
    }
}
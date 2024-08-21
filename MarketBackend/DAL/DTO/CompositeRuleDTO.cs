using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketBackend.Domain.Market_Client;

namespace MarketBackend.DAL.DTO
{
    public class CompositeRuleDTO : RuleDTO
    {
        public List<RuleDTO> Rules { get; set; }
        public string Operator { get; set; }
        public CompositeRuleDTO() { }
        public CompositeRuleDTO(RuleSubjectDTO subject, List<RuleDTO> rules, string op) : base(subject) {
            Rules = rules;
            Operator = op;
        }
        public CompositeRuleDTO(CompositeRule rule) : base(rule) {
            Rules = new List<RuleDTO>();
            foreach(IRule rulee in rule.Rules) {
                Rules.Add(DBcontext.GetInstance().Rules.Find(rulee.Id));
            }
            Operator = rule.Operator.ToString();
        }
        
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MarketBackend.Domain.Market_Client;


namespace MarketBackend.DAL.DTO{
    public class DiscountCompositePolicyDTO : DiscountPolicyDTO
    {
        public string NumericOperator { get; set; }
        public List<PolicyDTO> Policies { get; set; }
        public DiscountCompositePolicyDTO() { }
        public DiscountCompositePolicyDTO(int id, DateTime expirationDate, int ruleId, PolicySubjectDTO subject, double percentage, string numericOperator, List<PolicyDTO> policies) : base(id,expirationDate, ruleId, subject, percentage)
        {
            NumericOperator = numericOperator;
            Policies = policies;
        }
        public DiscountCompositePolicyDTO(DiscountCompositePolicy policy): base(policy)
        {
            NumericOperator = policy.NumericOperator.ToString();
            Policies = new List<PolicyDTO>();
            foreach (IPolicy subPolicy in policy.Policies)
                Policies.Add(subPolicy.CloneDTO());
        }
    }
}
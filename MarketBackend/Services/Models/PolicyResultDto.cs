using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketBackend.Domain.Market_Client;

namespace MarketBackend.Services.Models
{
    public class PolicyResultDto
    {
        public int Id { get;set; }
        public DateTime ExpirationDate { get;set; }
        public int StoreId { get;set; }
        public RuleResultDto Rule { get;set; }
        public ProductResultDto Product { get;set; }
        public string Category { get;set; }
    
        public PolicyResultDto(IPolicy policy)
        {
            Id = policy.Id;
            ExpirationDate = policy.ExpirationDate;
            StoreId = policy.StoreId;
            Rule = new(policy.Rule);
            if (policy.Subject.Product != null)
                Product = new(policy.Subject.Product);
            else
                Product = null;
            Category = policy.Subject.Category;
        }
    }

    public class DiscountPolicyResultDto : PolicyResultDto
    {
        public DiscountPolicyResultDto(DiscountPolicy policy) : base(policy)
        {
            Precentage = policy.Precentage;
        }

        public double Precentage { get; set; }
    }
}
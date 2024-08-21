using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MarketBackend.Domain.Market_Client;


namespace MarketBackend.DAL.DTO{
    public class DiscountPolicyDTO : PolicyDTO
    {
        public double Precentage { get; set; }
        public DiscountPolicyDTO() { }
        public DiscountPolicyDTO(int id,DateTime expirationDate, int ruleId, PolicySubjectDTO subject, double percentage): base(id,expirationDate, ruleId, subject)
        {
            Precentage = percentage;
        }
        public DiscountPolicyDTO(DiscountPolicy policy): base(policy)
        {
            Precentage = policy.Precentage;
        }
    }
}
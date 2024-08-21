using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceAPI.Models.Dtos
{
    public class PolicyDto
    {
        public DateTime ExpirationDate { get; set; }
        public string Subject { get; set; }
        public int RuleId { get; set; }
    }

    public class DiscountPolicyDto : PolicyDto
    {
        public double Precantage { get; set; }
    }

    public class CompositePolicyDto
    {
        public DateTime ExpirationDate { get; set; }
        public int Operator { get; set; }
        public string Subject { get; set; }
        public List<int> Policies { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace EcommerceAPI.Models.Dtos
{
    public class RuleDto
    {
        public string? Subject { get; set; }
        public bool IsValid()
        {            
            return !string.IsNullOrEmpty(Subject);
        }
    }

    public class QuantityRuleDto : RuleDto
    {
        public int MinQuantity { get; set; }
        public int MaxQuantity { get; set; }
    }

    public class TotalPriceRuleDto : RuleDto
    {
        public int TargetPrice { get; set; }

    }

    public class CompositeRuleDto
    {
        public bool IsValid()
        {            
            return !Rules.IsNullOrEmpty();
        }
        public int Operator { get; set; }
        public List<int> Rules { get; set; }
    }
}
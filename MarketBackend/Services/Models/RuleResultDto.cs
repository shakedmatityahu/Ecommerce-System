using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketBackend.Domain.Market_Client;

namespace MarketBackend.Services.Models
{
    public class RuleResultDto
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public string SubjectInfo { get; set; }

        public RuleResultDto(IRule rule)
        {
            Id = rule.Id;
            StoreId = rule.storeId;
            SubjectInfo = rule.Subject.GetInfo();
        }
    }
}

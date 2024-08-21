using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MarketBackend.Domain.Market_Client;


namespace MarketBackend.DAL.DTO{
    [Table("Policies")]
    public class PolicyDTO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public DateTime ExpirationDate { get; set; }
        [ForeignKey("RuleDTO")]
        public int RuleId { get; set; }
        public PolicySubjectDTO PolicySubject { get; set; }


        public PolicyDTO() { }

        public PolicyDTO(int id,DateTime expirationDate, int ruleId, PolicySubjectDTO subject)
        {
            Id = id;
            ExpirationDate = expirationDate;
            RuleId = ruleId;
            PolicySubject = subject;
        }
        public PolicyDTO(IPolicy policy)
        {
            Id = policy.Id;
            ExpirationDate = policy.ExpirationDate;
            RuleId = policy.Rule.Id;
            PolicySubject = new PolicySubjectDTO(policy.Subject);
        }
    }
}

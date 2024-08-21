using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MarketBackend.Domain.Market_Client;

namespace MarketBackend.DAL.DTO
{
    [Table("Rules")]
    public class RuleDTO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public RuleSubjectDTO Subject { get; set; }
        public RuleDTO(RuleSubjectDTO subject)
        {
            Subject = subject;
        }
        public RuleDTO() { }
        public RuleDTO(RuleSubject subject) 
        {
            Subject = new RuleSubjectDTO(subject);
        }

        public RuleDTO(IRule rule)
        {
            Id = rule.Id;
            Subject = new RuleSubjectDTO(rule.Subject);
        }
    }
}
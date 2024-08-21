using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MarketBackend.Domain.Market_Client;


namespace MarketBackend.DAL.DTO{
    public class PurchasePolicyDTO: PolicyDTO
    {
        public PurchasePolicyDTO() { }
        public PurchasePolicyDTO(int id,DateTime expirationDate,int ruleId, PolicySubjectDTO subject): base(id, expirationDate, ruleId, subject) { }

        public PurchasePolicyDTO(PurchasePolicy policy): base(policy) { }
    }
}
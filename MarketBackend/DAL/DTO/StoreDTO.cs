using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MarketBackend.Domain.Models;
using MarketBackend.Domain.Market_Client;

namespace MarketBackend.DAL.DTO
{
  
    [Table("Stores")]
    public class StoreDTO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNum { get; set; }
        public string Email { get; set; }
        public bool Active { get; set; }
        public double Rating { get; set; }
        public List<ProductDTO> Products { get; set; }
        public List<PurchaseDTO> Purchases { get; set; }
        public List<RuleDTO> Rules { get; set; }
        public List<PolicyDTO> Policies { get; set; }

        public StoreDTO(int id, string name, string phoneNum, string email, bool active, double rating, List<ProductDTO> products, List<PurchaseDTO> purchases, List<RuleDTO> rules, List<PolicyDTO> policies)
        {
            Id = id;
            Name = name;
            Active = active;
            Email = email;
            PhoneNum = phoneNum;
            Rating = rating;
            Purchases = purchases;
            Products = products;
            Rules = rules;
            Policies = policies;

        }

        public StoreDTO(int id, string name, string phoneNum, string email,  bool active, double rating)
        {
            Id = id;
            Name = name;
            Active = active;
            Email =email;
            PhoneNum = phoneNum;
            Rating = rating;
            Purchases= new List<PurchaseDTO>();
            Products = new List<ProductDTO>();
            Rules = new List<RuleDTO>();
            Policies = new List<PolicyDTO>();
            
        }
        public StoreDTO(Store store)
        {
            Id = store.StoreId;
            Name = store.Name;
            Active = store.Active;
            Rating = store._raiting;
            Email = store._storeEmailAdd;
            PhoneNum = store._storePhoneNum;
            Purchases= new List<PurchaseDTO>();
            foreach (Purchase purchase in store._history._purchases)
                Purchases.Add(new PurchaseDTO(purchase));
            Products = new List<ProductDTO>();
            foreach (Product product in store.Products)
                Products.Add(new ProductDTO(product));
            Rules = new List<RuleDTO>();
            foreach (IRule rule in store._rules.Values)
                Rules.Add(rule.CloneDTO());
            Policies = new List<PolicyDTO>();
            foreach (IPolicy policy in store._purchasePolicyManager.Policies.Values)
                Policies.Add(policy.CloneDTO());
            foreach (IPolicy policy in store._discountPolicyManager.Policies.Values)
                Policies.Add(policy.CloneDTO());

        }
    }
}
    

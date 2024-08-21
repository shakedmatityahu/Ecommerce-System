using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MarketBackend.Domain.Market_Client;


namespace MarketBackend.DAL.DTO{
    [Table("Policy Subjects")]
    public class PolicySubjectDTO
    {
        [Key]
        public int Id { get; set; }
        public ProductDTO Product { get; set; }
        public string Category { get; set; }
        public PolicySubjectDTO() { }
        public PolicySubjectDTO(ProductDTO product, string category)
        {
            Product = product;
            Category = category;
        }
        public PolicySubjectDTO(RuleSubject subject)
        {
            if (subject.Product != null)
            {
                Product = DBcontext.GetInstance().Products.Find(subject.Product.ProductId);
            }
            else
            {
                Product = GenerateDummyProduct();
            }
            Category = subject.Category.ToString();
        }
        private ProductDTO GenerateDummyProduct()
        {
            if (ProductRepositoryRAM.GetInstance().ContainsID(-1))
                return DBcontext.GetInstance().Products.Find(-1);
            return new ProductDTO(-1,"null",-1,-1, "null", "None", "null");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MarketBackend.Domain.Market_Client;

namespace MarketBackend.DAL.DTO
{
     [Table("Rule Subjects")]
    public class RuleSubjectDTO
    {
        [Key]
        public int Id { get; set; }
        public ProductDTO Product { get; set; }
        public string Category { get; set; }

        public RuleSubjectDTO()
        {
        }

        public RuleSubjectDTO(ProductDTO product, string category)
        {
            Product = product;
            Category = category;
        }
        public RuleSubjectDTO(RuleSubject subject)
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
            ProductDTO dummyProductDTO = DBcontext.GetInstance().Products.Find(-1);
            if (dummyProductDTO != null)
                return dummyProductDTO;
            else
            {
                //need to check nofar
                dummyProductDTO = new ProductDTO(-1, "null", 1, 1, "None", "","", "RegularSell", 0);
                DBcontext.GetInstance().Products.Add(dummyProductDTO);
                DBcontext.GetInstance().SaveChanges();
                return dummyProductDTO;
            }

        }

    }
}
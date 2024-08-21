using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MarketBackend.Domain.Market_Client;


namespace MarketBackend.DAL.DTO{
    public class BasketItemDTO
    {
        [Key]
        public int Id { get; set; }
        public ProductDTO Product { get; set; }
        public double PriceBeforeDiscount { get; set; }
        public double PriceAfterDiscount { get; set; }
        public int Quantity { get; set; }

        public BasketItemDTO(ProductDTO product, double priceAfterDiscount, double priceBeforeDiscount, int quantity)
        {
            Product = product;
            PriceBeforeDiscount = priceBeforeDiscount;
            PriceAfterDiscount = priceAfterDiscount;
            Quantity = quantity;
        }
        public BasketItemDTO() { }
        public BasketItemDTO(BasketItem item) {
            Product = DBcontext.GetInstance().Products.Find(item.Product.ProductId);
            PriceBeforeDiscount = item.Product.Price;
            PriceAfterDiscount = item.PriceAfterDiscount;
            Quantity = item.Quantity;
        }
    }
}
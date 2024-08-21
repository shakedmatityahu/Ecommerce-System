using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketBackend.DAL;
using MarketBackend.Domain.Market_Client;

namespace MarketBackend.Services.Models
{
    public class BasketResultDto
    {
        public int StoreId { get; set; }
        public List<ProductResultDto> Products { get; set; }
        public double Price { get; set; }

        public BasketResultDto(Basket basket){
            StoreId = basket._storeId;
            var store = StoreRepositoryRAM.GetInstance().GetById(StoreId);
            Price = store.CalculateBasketPrice(basket);
            Products = store.Products.Where(product => basket.products.Keys.Contains(product.ProductId))
                .Select(product => new ProductResultDto(product){Quantity = basket.products[product.ProductId]}).ToList();
        }
    }
}
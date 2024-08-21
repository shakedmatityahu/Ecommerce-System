using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MarketBackend.Domain.Market_Client;


namespace MarketBackend.DAL.DTO
{
    [Table("Baskets")]
    public class BasketDTO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int BasketId { get; set; }
        public int StoreId { get; set; }
        public int CartId {get; set;}
        public List<BasketItemDTO> BasketItems { get; set; }
        public List<ProductDTO> Products { get; set; }


        public BasketDTO(int shopId, List<BasketItemDTO> basketItems)
        {
            StoreId = shopId;
            BasketItems = basketItems;
        }

        public double TotalPrice { get; set; }
        public BasketDTO() { }
        public BasketDTO(Basket basket) {
            BasketId = basket._basketId;
            StoreId = basket._storeId;
            CartId = basket._cartId;
            BasketItems = new List<BasketItemDTO>();
            Products = new List<ProductDTO>();
            foreach (BasketItem item in basket.BasketItems){
                BasketItems.Add(new BasketItemDTO(item));
                Products.Add(new ProductDTO(item.Product));
            } 

        }

    }
}
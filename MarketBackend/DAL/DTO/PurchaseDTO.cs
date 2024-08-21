using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MarketBackend.Domain.Market_Client;

namespace MarketBackend.DAL.DTO
{
    [Table("Purchases")]
    public class PurchaseDTO
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public int StoreId { get; set; }
        [ForeignKey("Baskets")]
        public int BasketId { get; set; }
        public string Identifierr { get; set; }
        public double Price { get; set; }

        public PurchaseDTO(Purchase purchase) {
            Id = purchase.PurchaseId;
            StoreId = purchase.StoreId;
            BasketId = purchase.Basket._basketId;
            Identifierr = purchase.Identifier;
            Price = purchase.Price;
        }

        public PurchaseDTO(Purchase purchase, BasketDTO basketDTO) {
            Id = purchase.PurchaseId;
            StoreId = purchase.StoreId;
            BasketId = basketDTO.BasketId;
            Identifierr = purchase.Identifier;
            Price = purchase.Price;
        }

        public PurchaseDTO() { }
         public PurchaseDTO(int id, int storeId, BasketDTO basket, string identifierr, double price)
        {
            Id = id;
            StoreId = storeId;
            BasketId = basket.BasketId;
            Identifierr = identifierr;
            Price = price;
        }
        
    }
}
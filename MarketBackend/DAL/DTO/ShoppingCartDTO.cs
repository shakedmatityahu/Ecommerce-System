using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MarketBackend.Domain.Market_Client;
using MarketBackend.Domain.Models;
using MarketBackend.Services.Interfaces;

namespace MarketBackend.DAL.DTO
{
    [Table("ShoppingCart")]
    public class ShoppingCartDTO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int  _shoppingCartId { get; set; }
        
        public List<BasketDTO> Baskets { get; set; }

        public ShoppingCartDTO(int id, List<Basket> baskets)
        {
            _shoppingCartId = id;
            Baskets = new List<BasketDTO>();
            foreach (Basket basket in baskets)
                Baskets.Add(new BasketDTO(basket));
        }
        public ShoppingCartDTO(int id)
        {
            _shoppingCartId = id;
            Baskets = new List<BasketDTO>();
        }
        public ShoppingCartDTO(){ }
        public ShoppingCartDTO(ShoppingCart cart) {
            _shoppingCartId = cart. _shoppingCartId;
            Baskets = new List<BasketDTO>();
            foreach (Basket basket in BasketRepositoryRAM.GetInstance().getBasketsByCartId(_shoppingCartId))
                Baskets.Add(new BasketDTO(basket));
        }
    }
}
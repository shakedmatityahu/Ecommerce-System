using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketBackend.Domain.Market_Client
{
    public class Guest : Client
    {
        public Guest(int id) : base(id)
        {
        }

        public override void AddToCart(int storeId, int productId, int quantity){
            Cart.addToCartGuest(storeId, productId, quantity);
        }

        public override void RemoveFromCart(int storeId, int productId, int quantity){
            Cart.removeFromCartGuest(storeId, productId, quantity);
        }
    }
}
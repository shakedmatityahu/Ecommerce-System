using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Diagnostics.Contracts;
using MarketBackend.Domain.Models;

namespace MarketBackend.Domain.Market_Client
{
    public abstract class Client
    {
        public int Id  {get; set;}
        public ShoppingCart Cart {get; set;}
        public bool IsAbove18 {get; set;}

        public Client(int clientId){
            Id = clientId;
            Cart = new ShoppingCart(Id);
            IsAbove18 = false;
        }

        public virtual void AddToCart(int storeId, int productId, int quantity){
            Cart.addToCart(storeId, productId, quantity);
        }

        public virtual void RemoveFromCart(int storeId, int productId, int quantity){
            Cart.removeFromCart(storeId, productId, quantity);
        }

        public virtual bool ResToStoreManagerReq(){
            return true;
        }

        public virtual bool ResToStoreOwnershipReq(){
            return true;
        }

        public virtual void PurchaseBasket(Basket basket)
        {            
            Cart.PurchaseBasket(basket._basketId);
        }
    }
}
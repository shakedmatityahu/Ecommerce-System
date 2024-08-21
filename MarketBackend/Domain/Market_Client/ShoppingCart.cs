using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketBackend.DAL;
using MarketBackend.DAL.DTO;
using MarketBackend.Domain.Market_Client;
using MarketBackend.Services.Interfaces;

namespace MarketBackend.Domain.Models
{
    public class ShoppingCart
    {
        public int _shoppingCartId;
        private readonly IBasketRepository _basketRepository;
        public ShoppingCart(int shoppingCartId){
            _shoppingCartId = shoppingCartId;
            _basketRepository = BasketRepositoryRAM.GetInstance();
        }

        public void addToCart(int storeId, int productId, int quantity){
            Basket? basket = GetBaskets().Values.Where(basket => basket._storeId == storeId).FirstOrDefault() ?? _basketRepository.CreateBasket(storeId, _shoppingCartId);
            basket.addToBasket(productId, quantity);
        }

        public void addToCartGuest(int storeId, int productId, int quantity){
            Basket? basket = GetBaskets().Values.Where(basket => basket._storeId == storeId).FirstOrDefault() ?? _basketRepository.CreateBasketGuest(storeId, _shoppingCartId);
            basket.addToBasketGuest(productId, quantity);
        }

        public void removeFromCart(int storeId, int productId, int quantity){
            var basket = GetBaskets().Values.Where(basket => basket._storeId == storeId).FirstOrDefault();
            basket.RemoveFromBasket(productId, quantity);
        }

        public void removeFromCartGuest(int storeId, int productId, int quantity){
            var basket = GetBaskets().Values.Where(basket => basket._storeId == storeId).FirstOrDefault();
            basket.RemoveFromBasketGuest(productId, quantity);
        }

        public Dictionary<int, Basket> GetBaskets() //returns a dictionary of store id to productIdxQuantity
        {
            var baskets = _basketRepository.getBasketsByCartId(_shoppingCartId);
            var retBaskets = new Dictionary<int, Basket>();
            foreach (var basket in baskets) {
                retBaskets.Add(basket._storeId, basket);
            };
            return retBaskets;
        }

        public void PurchaseBasket(int basketId){
            Basket basket = _basketRepository.GetById(basketId);
            _basketRepository.Delete(basket);
        }
    }

    public class ShoppingCartHistory
    {
        public int _shoppingCartId{get; set;}
        public ConcurrentDictionary<int, Basket> _baskets = new();
        public ConcurrentDictionary<int, Product> _products = new();

        public ShoppingCartHistory(){}
        public ShoppingCartHistory(ShoppingCartHistoryDTO other)
        {
            _shoppingCartId = other.ShoppingCartId;
            _baskets = new(other._baskets.Select(basket => new Basket(basket)).ToDictionary(basket => basket._storeId));
            _products = new(other._products.Select(product => new Product(product)).ToDictionary(product => product.ProductId));
        }

        public void AddBasket(Basket basket)
        {
            _baskets.TryAdd(basket._basketId, Basket.Clone(basket));
            foreach(var product in basket.products)
            {
                var storeProducts = ProductRepositoryRAM.GetInstance().GetStoreProducts(basket._storeId);
                var productDetailsFromStore = storeProducts.Where(p => p.ProductId == product.Key).FirstOrDefault().Clone();
                if(productDetailsFromStore is not null) _products.TryAdd(product.Key, productDetailsFromStore);
            }
            
        }
    }
}
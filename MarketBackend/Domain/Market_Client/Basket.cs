using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketBackend.DAL;
using MarketBackend.DAL.DTO;
using MarketBackend.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace MarketBackend.Domain.Market_Client
{
    public class Basket
    {
        public int _basketId{get; set;}
        public int _storeId{get; set;}
        public int _cartId{get; set;}
        public Dictionary<int, int> products{get;} //product id to quantity
        private SynchronizedCollection<BasketItem> _basketItems;
        public SynchronizedCollection<BasketItem> BasketItems { get => _basketItems; set => _basketItems = value; }
        private object Lock;

        public Basket(int basketId, int storeId){
            _basketId = basketId; 
            _storeId = storeId;
            products = new Dictionary<int, int>();
            _basketItems = new SynchronizedCollection<BasketItem>();
            Lock = new object();
        }

        public Basket(BasketDTO other)
        {
            _basketId = other.BasketId;
            _storeId = other.StoreId;
            _basketItems = new(other.BasketItems);
            List<ProductDTO> productDTOs = other.Products;
            foreach (ProductDTO productDTO in productDTOs)
            {
                Product product = new Product(productDTO);
                products[product.ProductId] = productDTO.Quantity;
            }         
        }

        public void addToBasket(int productId, int quantity){
            if (products.ContainsKey(productId)){
                products[productId] += quantity;
                FindBasketItem(productId).Quantity += quantity;
                int quantityInBasket = FindBasketItem(productId).Quantity;
                lock (Lock)
                {
                    DBcontext dBcontext = DBcontext.GetInstance();
                    BasketDTO basketDTO = dBcontext.Baskets.Find(_basketId);
                    BasketItemDTO basketItemDTO = basketDTO.BasketItems.FirstOrDefault(bi => bi.Product.ProductId == productId);
                    if (basketItemDTO != null)
                    {
                        basketItemDTO.Quantity = quantityInBasket;
                        dBcontext.SaveChanges();
                    }
                }
            }
            else{
                products[productId] = quantity;
                BasketItem basketItem = new BasketItem(ProductRepositoryRAM.GetInstance().GetById(productId), quantity);
                _basketItems.Add(basketItem);
                lock (Lock)
                {
                    DBcontext dBcontext = DBcontext.GetInstance();
                    BasketItemDTO basketItemDTO = new BasketItemDTO(basketItem);
                    // dBcontext.BasketItems.Add(basketItemDTO);
                    BasketDTO basketDTO = dBcontext.Baskets.Find(_basketId);
                    basketDTO.BasketItems.Add(basketItemDTO);
                    dBcontext.SaveChanges();
                }
            }
        }

        public void addToBasketGuest(int productId, int quantity){
            if (products.ContainsKey(productId)){
                products[productId] += quantity;
                FindBasketItem(productId).Quantity += quantity;
            }
            else{
                products[productId] = quantity;
                BasketItem basketItem = new BasketItem(ProductRepositoryRAM.GetInstance().GetById(productId), quantity);
                _basketItems.Add(basketItem);
            }
        }

        public void RemoveFromBasket(int productId, int quantity){
            if (products.ContainsKey(productId)){
                products[productId] = Math.Max(products[productId]-quantity,0);
                if (products[productId] == 0) {
                    products.Remove(productId);
                    _basketItems.Remove(FindBasketItem(productId));
                    lock (Lock)
                    {
                        DBcontext dBcontext = DBcontext.GetInstance();
                        BasketDTO basketDTO = dBcontext.Baskets.Find(_basketId);
                        BasketItemDTO basketItemDTO = basketDTO.BasketItems.FirstOrDefault(bi => bi.Product.ProductId == productId);
                        if (basketItemDTO != null)
                        {
                            dBcontext.BasketItems.Remove(basketItemDTO);
                            dBcontext.Baskets.Remove(basketDTO);
                            dBcontext.SaveChanges();
                        }
                    }
                }
                else{
                    FindBasketItem(productId).Quantity -= quantity;
                    int quantityInBasket = FindBasketItem(productId).Quantity;
                    lock (Lock)
                    {
                        DBcontext dBcontext = DBcontext.GetInstance();
                        BasketDTO basketDTO = dBcontext.Baskets.Find(_basketId);
                        BasketItemDTO basketItemDTO = basketDTO.BasketItems.FirstOrDefault(bi => bi.Product.ProductId == productId);
                        if (basketItemDTO != null)
                        {
                            basketItemDTO.Quantity = quantityInBasket;
                            dBcontext.SaveChanges();
                        }
                    }
                }
            }
            else{
                throw new ArgumentException($"Product id={productId} not in the {_basketId}!");
            }
        }

        public void RemoveFromBasketGuest(int productId, int quantity){
            if (products.ContainsKey(productId)){
                products[productId] = Math.Max(products[productId]-quantity,0);
                if (products[productId] == 0) {
                    products.Remove(productId);
                    _basketItems.Remove(FindBasketItem(productId));
                }
                else{
                    FindBasketItem(productId).Quantity -= quantity;
                }
            }
            else{
                throw new ArgumentException($"Product id={productId} not in the {_basketId}!");
            }
        }

        public BasketItem GetBasketItem(Product product)
        {
            foreach(BasketItem basketItem in _basketItems)
            {
                if (basketItem.Product == product)
                {
                    return basketItem;
                }
            }
            return null;
        }

        public BasketItem FindBasketItem(int productId)
        {
            foreach (BasketItem basketItem in _basketItems)
            {
                if (basketItem.Product.ProductId == productId) return basketItem;
            }
            return null;
        }

        public static Basket Clone(Basket basketToClone)
        {
            var newBasket = new Basket(basketToClone._basketId, basketToClone._storeId)
            {
                _cartId = basketToClone._cartId
            };            

            foreach (var product in basketToClone.products)
            {
                newBasket.products[product.Key] = product.Value;
            }

            return newBasket;
        }

        //TODO:
        public string GetInfo()
        {
            throw new NotImplementedException();
        }

        public bool IsEmpty()
        {
            return products.IsNullOrEmpty();
        }
        public bool HasProduct(Product p)
        {
            foreach(BasketItem basketItem in _basketItems)
            {
                if(basketItem.Product == p) return true;
            }
            return false;
        }   
        public double GetBasketPriceBeforeDiscounts()
        {
            double price = 0;
            foreach (BasketItem basketItem in _basketItems)
                price += basketItem.Product.Price * basketItem.Quantity;
            return price;
        }
        public void resetDiscount()
        {
            foreach (BasketItem basketItem in _basketItems)
            {
                if(basketItem.Product._sellMethod is RegularSell)
                    basketItem.PriceAfterDiscount = basketItem.Product.Price;
            }
        }
    }
}
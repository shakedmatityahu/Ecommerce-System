using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MarketBackend.DAL.DTO;
using MarketBackend.Domain.Market_Client;
using MarketBackend.Domain.Models;
using MarketBackend.Services.Interfaces;

namespace MarketBackend.DAL
{
    public class BasketRepositoryRAM : IBasketRepository
    {
        private readonly ConcurrentDictionary<int, Basket> baskets;
        private int BasketCounter{get; set;}

        private static BasketRepositoryRAM _basketRepository = null;
        private object Lock;

        private BasketRepositoryRAM()
        {
            baskets = new ConcurrentDictionary<int, Basket>();
            BasketCounter = 1;
            Lock = new object();
        }
        public static BasketRepositoryRAM GetInstance()
        {
            _basketRepository ??= new BasketRepositoryRAM();
            return _basketRepository;
        }

        public static void Dispose(){
            _basketRepository = new BasketRepositoryRAM();
        }
        public Basket CreateBasket(int storeId, int cartId)
        {
            var newBasket = new Basket(BasketCounter, storeId)
            {
                _cartId = cartId
            };
            baskets.TryAdd(newBasket._basketId, newBasket);
            try{
                DBcontext dbContext = DBcontext.GetInstance();
                lock (Lock)
                {
                    dbContext.Baskets.Add(new BasketDTO(newBasket));
                    dbContext.SaveChanges();
                }
            }
            catch(Exception){
                throw new Exception("There was a problem in Database use- Create Basket");
            }
            
            BasketCounter ++;
            return newBasket;
        }

        public Basket CreateBasketGuest(int storeId, int cartId)
        {
            var newBasket = new Basket(BasketCounter, storeId)
            {
                _cartId = cartId
            };
            baskets.TryAdd(newBasket._basketId, newBasket);            
            BasketCounter ++;
            return newBasket;
        }

        public void Add(Basket entity)
        {
            if(baskets.ContainsKey(entity._basketId)){
                throw new ArgumentException($"Basket with ID {entity._basketId} already exists.");

            }
            try{
                DBcontext dbContext = DBcontext.GetInstance();
                baskets.TryAdd(entity._basketId, entity);
                lock (Lock)
                {
                    dbContext.Baskets.Add(new BasketDTO(entity));
                    dbContext.SaveChanges();
                }
            }
            catch(Exception){
                throw new Exception("There was a problem in Database use- Add Basket");
            }
            

        }

        public void Delete(Basket entity)
        {
            if (baskets.ContainsKey(entity._basketId)){
                baskets.TryRemove(new KeyValuePair<int, Basket>(entity._basketId, entity));
            }
            else{
                throw new KeyNotFoundException($"Basket with ID {entity._basketId} does not exist.");
            }
            try{
                lock (Lock)
                {
                    var dbContext = DBcontext.GetInstance();
                    var dbBasket = dbContext.Baskets.Find(entity._basketId);
                    if (dbBasket is not null){
                        dbContext.Baskets.Remove(dbBasket);
                        dbContext.SaveChanges();
                    }
                }
            }
            catch(Exception){
                throw new Exception("There was a problem in Database use- Delete Basket");
            }
            
        }

        public IEnumerable<Basket> getAll()
        {
            return baskets.Values.ToList();
        }

        public IEnumerable<Basket> getBasketsByCartId(int cartId)
        {
            return baskets.Values.Where(basket => basket._cartId == cartId).ToList();
        }

        public Basket GetById(int id){
            if (baskets.ContainsKey(id)){
                return baskets[id];
            }
            else
            {
                try{
                    var dbContext = DBcontext.GetInstance();
                    BasketDTO bDto = dbContext.Baskets.Find(id);
                    if (bDto != null)
                    {
                        LoadBasket(bDto);
                        return baskets[id];
                    }
                    throw new ArgumentException("Invalid user ID.");
                }
                catch(Exception){
                throw new Exception("There was a problem in Database use- Get Basket");
                }
                
            }
        }

        public Basket? TryGetById(int id){

            baskets.TryGetValue(id, out Basket? value);
            return value;
        }

        public void Update(Basket entity)
        {
            if (baskets.ContainsKey(entity._basketId))
            {
                try{
                    baskets[entity._basketId] = entity;
                    lock(Lock){
                        BasketDTO p = DBcontext.GetInstance().Baskets.Find(entity._basketId);
                        if (p != null){
                            if (entity.BasketItems!= null){
                                List<BasketItemDTO> BasketItems = new List<BasketItemDTO>();
                                List<ProductDTO> Products = new List<ProductDTO>();
                                foreach (BasketItem item in entity.BasketItems){
                                BasketItems.Add(new BasketItemDTO(item));
                                Products.Add(new ProductDTO(item.Product));
                                }
                                p.BasketItems = BasketItems;
                                p.Products = Products;
                            }
                            DBcontext.GetInstance().SaveChanges();
                        }
                    }
                }
                catch(Exception){
                throw new Exception("There was a problem in Database use- Update Basket");
                }
                
            }
            else
            {
                throw new KeyNotFoundException($"Basket with ID {entity._basketId} not found.");
            }
        }

        private void Load()
        {
            var dbContext = DBcontext.GetInstance();
            List<BasketDTO> Lbaskets = dbContext.Baskets.ToList();
            foreach (BasketDTO basket in Lbaskets)
            {
                baskets.TryAdd(basket.BasketId, new Basket(basket));
            }
        }

        private void LoadBasket(BasketDTO basketDTO)
        {
            Basket basket = new Basket(basketDTO);
            baskets[basket._basketId] = basket;
        }

        public void Add_cartHistory(ShoppingCartHistory shoppingCartHistory, string memberUserName)
        {
            try{
                lock (Lock){
                var dbContext = DBcontext.GetInstance();
                ShoppingCartHistoryDTO shoppingCartHistoryDTO = new ShoppingCartHistoryDTO(shoppingCartHistory);
                MemberDTO memberDTO = dbContext.Members.Where(member => member.UserName == memberUserName).ToList()[0];
                memberDTO.OrderHistory.Add(shoppingCartHistoryDTO);
                dbContext.SaveChanges();
                }
            }
            catch(Exception){
                throw new Exception("There was a problem in Database use- Add Cart History");
            }
        }
    }
}

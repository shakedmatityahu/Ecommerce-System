using MarketBackend.Domain.Models;
using MarketBackend.Services.Interfaces;
using MarketBackend.Domain.Market_Client;
using MarketBackend.DAL.DTO;
using Microsoft.EntityFrameworkCore;

namespace MarketBackend.DAL
{
    public class PurchaseRepositoryRAM : IPurchaseRepository
    {
        private static Dictionary<int, Purchase> _purchaseById;

        private static PurchaseRepositoryRAM _purchaseRepo = null;
        private object _lock;


        private PurchaseRepositoryRAM()
        {
            _purchaseById = new Dictionary<int, Purchase>();
            _lock = new object();
        }
        public static PurchaseRepositoryRAM GetInstance()
        {
            if (_purchaseRepo == null)
                _purchaseRepo = new PurchaseRepositoryRAM();
            return _purchaseRepo;
        }

        public static void Dispose(){
            _purchaseRepo = new PurchaseRepositoryRAM();
        }
        public void Add(Purchase purchase)
        {
            try{
                lock (_lock){
                _purchaseById.Add(purchase.PurchaseId, purchase);
                DBcontext context = DBcontext.GetInstance();
                StoreDTO storeDTO = new StoreDTO(StoreRepositoryRAM.GetInstance().GetById(purchase.StoreId));
                // StoreDTO storeDTO = context.Stores.Include(s => s.Purchases).FirstOrDefault(s => s.Id == purchase.StoreId);
                // BasketDTO basketDTO = context.Baskets.Find(purchase.Basket._basketId);
                BasketDTO basketDTO = new BasketDTO(BasketRepositoryRAM.GetInstance().GetById(purchase.Basket._basketId));
                PurchaseDTO purchaseDTO = new PurchaseDTO(purchase, basketDTO);
                storeDTO.Purchases.Add(purchaseDTO);
                context.Purchases.Add(purchaseDTO);
                context.SaveChanges();
            }
            }
            catch(Exception){
                throw new Exception("There was a problem in Database use- Add Purchase");
            }
            
            
        }
        public Purchase GetById(int id)
        {
            if (_purchaseById.ContainsKey(id))
                return _purchaseById[id];
            else
            {
                try{
                    lock (_lock)
                    {
                        PurchaseDTO purchaseDTO = DBcontext.GetInstance().Purchases.Find(id);
                        if (purchaseDTO != null)
                        {
                            _purchaseById.Add(id, new Purchase(purchaseDTO));
                        }
                        return _purchaseById[id];
                    }
                }
                catch(Exception){
                throw new Exception("There was a problem in Database use- Get Purchase");
                }
                
            }
        }
    
        public void Delete(Purchase Purchase)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Purchase> getAll()
        {
            try{
                lock (_lock)
                {
                    foreach (PurchaseDTO purchaseDTO in DBcontext.GetInstance().Purchases)
                    {
                        _purchaseById.TryAdd(purchaseDTO.Id, new Purchase(purchaseDTO));
                    }
                }
                return _purchaseById.Values.ToList();
            }
            catch(Exception){
                throw new Exception("There was a problem in Database use- Get all Purchases");
            }
            
        }

        public void Update(Purchase purchase)
        {
            if (_purchaseById.ContainsKey(purchase.PurchaseId)){
                _purchaseById[purchase.PurchaseId] = purchase;
                try{
                    lock (_lock)
                    {
                        DBcontext context = DBcontext.GetInstance();
                        PurchaseDTO purchaseDTO = context.Purchases.Find(purchase.PurchaseId);
                        if (purchaseDTO != null){
                            purchaseDTO.Price = purchase.Price;
                        }
                        context.SaveChanges();
                    }
                }
                catch(Exception){
                throw new Exception("There was a problem in Database use- Update Purchases");
                }
                
            }
            else{
                throw new KeyNotFoundException($"Purchase with ID {purchase.PurchaseId} not found.");
            }
            
             
           
        }
    

        public SynchronizedCollection<Purchase> GetShopPurchaseHistory(int storeId)
        {
            SynchronizedCollection<Purchase> result = new SynchronizedCollection<Purchase>();
            try{
                lock (_lock)
                {
                    List<PurchaseDTO> lp = DBcontext.GetInstance().Purchases.Where((p) => p.StoreId == storeId).ToList();
                    foreach (PurchaseDTO purchaseDTO in lp)
                    {
                        _purchaseById.TryAdd(purchaseDTO.Id, new Purchase(purchaseDTO));
                    }
                }
            }
            catch(Exception){
                throw new Exception("There was a problem in Database use- Get Purchases History");
            }
            foreach (Purchase purchase in _purchaseById.Values)
            {
                if (purchase.StoreId == storeId) result.Add(purchase);
            }
            return result;
        }
    }
}
using MarketBackend.Domain.Models;
using MarketBackend.Services.Interfaces;
using MarketBackend.Domain.Market_Client;
using System.Collections.Concurrent;
using MarketBackend.DAL.DTO;

namespace MarketBackend.DAL
{
    public class StoreRepositoryRAM : IStoreRepository
    {
          private static ConcurrentDictionary<int, Store> _stores;
        private static StoreRepositoryRAM StoreRepository = null;

        public ConcurrentDictionary<int, Store> Stores { get => _stores; set => _stores = value; }
        private object _lock ;

        private StoreRepositoryRAM()
        {
            _stores = new ConcurrentDictionary<int, Store>();
            _lock = new object();
           
        }
        public static StoreRepositoryRAM GetInstance()
        {
            if (StoreRepository == null)
                StoreRepository = new StoreRepositoryRAM();
            return StoreRepository;
        }

        public static void Dispose(){
            StoreRepository = new StoreRepositoryRAM();
        }
        public void Add(Store store)
        {
            _stores.TryAdd(store.StoreId, store);
            try{
                lock(_lock){
                    DBcontext.GetInstance().Stores.Add(new StoreDTO(store));
                    DBcontext.GetInstance().SaveChanges();
                }
            }
            catch(Exception){
                throw new Exception("There was a problem in Database use- Add Store");
            }
        }

        public void Delete(Store store)
        {
        try{
            lock (_lock)
            {
                bool shopInDomain = _stores.TryRemove(store.StoreId, out _);
                DBcontext context = DBcontext.GetInstance();
                StoreDTO storeDTO = context.Stores.Find(store.StoreId);
                if (shopInDomain)
                {
                    context.Stores.Remove(storeDTO);
                    context.SaveChanges();
                }
                else if (storeDTO != null)
                {
                    context.Stores.Remove(storeDTO);
                    context.SaveChanges();
                }
            }
        }
        catch(Exception){
                throw new Exception("There was a problem in Database use- Delete Store");
        }

        
        }

        public IEnumerable<Store> getAll()
        {
            try{
                lock (_lock)
                {
                    List<StoreDTO> storesList = DBcontext.GetInstance().Stores.ToList();
                    foreach (StoreDTO storeDTO in storesList)
                    {
                        List<ProductDTO> products = DBcontext.GetInstance().Products.ToList();
                        foreach (ProductDTO productDTO in products)
                        {
                            if (productDTO.ProductId / 10 == storeDTO.Id){
                                storeDTO.Products.Add(productDTO);
                            }
                        }
                        List<PurchaseDTO> purchases = DBcontext.GetInstance().Purchases.ToList();
                        foreach (PurchaseDTO purchaseDTO in purchases)
                        {
                            if (purchaseDTO.StoreId == storeDTO.Id){
                                storeDTO.Purchases.Add(purchaseDTO);
                            }
                        }
                        _stores.TryAdd(storeDTO.Id, new Store(storeDTO));
                    }
                    
                }
            }
            catch(Exception){
                throw new Exception("There was a problem in Database use- Get all Stores");
            }
            
            
            return _stores.Values.ToList();
        }

        public Store GetById(int id)
        {
            if (_stores.ContainsKey(id))
            {
                return _stores[id];
            }
            else{
                try{
                    lock (_lock)
                    {
                        StoreDTO storeDTO = DBcontext.GetInstance().Stores.Find(id);
                        if (storeDTO != null)
                        {
                            Store store = new Store(storeDTO);
                            _stores.TryAdd(id, store);
                            store.Initialize(storeDTO);
                            return store;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                catch(Exception){
                throw new Exception("There was a problem in Database use- Get Store");
                }
                
            }
        }

        public void Update(Store store)
        {
            try{
                _stores[store._storeId] = store;
                lock(_lock){
                    StoreDTO storeDTO = DBcontext.GetInstance().Stores.Find(store._storeId);
                    StoreDTO newStore = new StoreDTO(store);
                    if (storeDTO != null)
                    {
                        storeDTO.Active = newStore.Active;
                        storeDTO.Purchases = newStore.Purchases;
                        storeDTO.Products = newStore.Products;
                        storeDTO.Rules = newStore.Rules;
                        storeDTO.Name = newStore.Name;
                        storeDTO.Rating = newStore.Rating;
                    }
                    else DBcontext.GetInstance().Stores.Add(newStore);
                    DBcontext.GetInstance().SaveChanges();
                }
            }
            catch(Exception){
                throw new Exception("There was a problem in Database use- Update Store");
            }
            
        }
    }
}
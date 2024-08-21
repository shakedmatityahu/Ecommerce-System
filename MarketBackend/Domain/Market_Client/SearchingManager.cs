using System.Collections.Concurrent;
using System.Diagnostics.Contracts;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using MarketBackend.DAL;
using MarketBackend.Domain.Models;

namespace MarketBackend.Domain.Market_Client{
    public static class SearchingManager
    {
        public static HashSet<Product> serachByNameWithStore(int storeId, string productName)
        {
            HashSet<Product> resultProducts = new HashSet<Product>();
            Store store = StoreRepositoryRAM.GetInstance().GetById(storeId);
            if (store == null)
            {
                return resultProducts;
            }
            return store.SearchByName(productName);
        }

        public static HashSet<Product> searchByCategoryWithStore(int storeId, string category)
        {
            HashSet<Product> resultProducts = new HashSet<Product>();
            Store store = StoreRepositoryRAM.GetInstance().GetById(storeId);
            if (store == null)
            {
                return resultProducts;
            }
            return store.SearchByCategory(category);
        }

        public static HashSet<Product> searchByKeywordWithStore(int storeId, string keyword)
        {
            HashSet<Product> resultProducts = new HashSet<Product>();
            Store store = StoreRepositoryRAM.GetInstance().GetById(storeId);
            if (store == null)
            {
                return resultProducts;
            }
            return store.SearchByKeywords(keyword);
        }     

        public static HashSet<Product> serachByName(string productName){
            HashSet<Product> resultProducts = new HashSet<Product>();
            List<Store> relevantstores = StoreRepositoryRAM.GetInstance().getAll().ToList();
            foreach (Store store in relevantstores)
            {
                resultProducts.UnionWith(serachByNameWithStore(store.StoreId, productName));
            }
            return resultProducts;
        }

        public static HashSet<Product> searchByCategory(string category){
            HashSet<Product> resultProducts = new HashSet<Product>();
            List<Store> relevantstores = StoreRepositoryRAM.GetInstance().getAll().ToList();
            foreach (Store store in relevantstores)
            {
                resultProducts.UnionWith(searchByCategoryWithStore(store.StoreId, category));
            }
            return resultProducts;
        }

        public static HashSet<Product> searchByKeyword(string keyword){
            HashSet<Product> resultProducts = new HashSet<Product>();
            List<Store> relevantstores = StoreRepositoryRAM.GetInstance().getAll().ToList();
            foreach (Store store in relevantstores)
            {
                resultProducts.UnionWith(searchByKeywordWithStore(store.StoreId, keyword));
            }
            return resultProducts;
        }
    }
}
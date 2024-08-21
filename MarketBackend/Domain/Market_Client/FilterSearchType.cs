using System.Collections.Concurrent;
using System.Text;

namespace MarketBackend.Domain.Market_Client{
    public abstract class FilterSearchType
    {
        public void Filter(HashSet<Product> products)
        {
            HashSet<Product> filteredProducts = new HashSet<Product>();
            foreach (Product product in products)
            {
                if (Predicate(product))
                {
                    filteredProducts.Add(product);
                }
            }
            products.Clear();
            products.UnionWith(filteredProducts);
        }
        protected abstract bool Predicate(Product product);
    }
}
using System.Collections.Concurrent;
using System.Text;

namespace MarketBackend.Domain.Market_Client
{
    public class CategoryFilter : FilterSearchType
    {
       private string _category;
        public CategoryFilter(string category)
        {
            _category = category;
        }

        protected override bool Predicate(Product product)
        {
            return (product.Category == _category) || (_category == "All");
        }
    
    }
}

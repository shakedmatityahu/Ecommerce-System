using System.Collections.Concurrent;
using System.Diagnostics.Contracts;
using System.Text;

namespace MarketBackend.Domain.Market_Client
{
    public class FilterParameterManager
    {
        private string _category;
        private double _lowPrice;
        private double _highPrice;
        private double _lowProductRate;
        private double _highProductRate;       
        private double _lowStoreRate;
        private double _highStoreRate;
        public FilterParameterManager(string category, double lowPrice, double highPrice, double lowProductRate, double highProductRate, double lowStoreRate, double highStoreRate)
        {
            _category = category;
            _lowPrice = lowPrice;
            _highPrice = highPrice;
            _lowProductRate = lowProductRate;
            _highProductRate = highProductRate;
            _lowStoreRate = lowStoreRate;
            _highStoreRate = highStoreRate;
        }
        public void Filter(HashSet<Product> products)
        {
            HashSet<FilterSearchType> filters = new HashSet<FilterSearchType>
            {
                new CategoryFilter(_category),
                new ProductPriceRangeFilter(_lowPrice, _highPrice),
                new ProductRatingFilter(_lowProductRate, _highProductRate),
                new StoreRatingFilter(_lowStoreRate, _highStoreRate)
            };
            foreach (FilterSearchType filter in filters)
            {
                filter.Filter(products);
            }
        }

    
    }
}
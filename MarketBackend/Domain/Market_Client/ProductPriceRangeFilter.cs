using System.Collections.Concurrent;
using System.Text;

namespace MarketBackend.Domain.Market_Client
{
    public class ProductPriceRangeFilter : FilterSearchType
    {
        private double _lowPrice;
        private double _highPrice;
        public ProductPriceRangeFilter(double lowPrice, double highPrice)
        {
            _lowPrice = lowPrice;
            _highPrice = highPrice;
        }

        protected override bool Predicate(Product product)
        {
            return (product.ProductRating >= _lowPrice) && (product.ProductRating <= _highPrice);
        }
    }
}

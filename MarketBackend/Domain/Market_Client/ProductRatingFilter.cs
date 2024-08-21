using System.Collections.Concurrent;
using System.Text;

namespace MarketBackend.Domain.Market_Client
{
    public class ProductRatingFilter : FilterSearchType
    {
        private double _lowRate;
        private double _highRate;
        public ProductRatingFilter(double lowRate, double highRate)
        {
            _lowRate = lowRate;
            _highRate = highRate;
        }

        protected override bool Predicate(Product product)
        {
            return (product.ProductRating >= _lowRate) && (product.ProductRating <= _highRate);
        }
    }
}

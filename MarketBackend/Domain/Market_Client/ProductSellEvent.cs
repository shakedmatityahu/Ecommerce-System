
using MarketBackend.Domain.Models;

namespace MarketBackend.Domain.Market_Client
{
    public class ProductSellEvent : Event
    {
        private Purchase _purchase;
        private Store _store;

        public ProductSellEvent(Store store, Purchase purchase) : base("Product Sell Event")
        {
            _store = store;
            _purchase = purchase;
        }

        public override string GenerateMsg()
        {
            string products = "";
            foreach (var product in _purchase.Basket.products){
                products += $"ProductId: {product.Key}, Quantity: {product.Value}. ";
            }
            return $"{Name}: Store: \'{_store._storeName}\', Products:" + $"{products}, " +
            $"Price: {_purchase.Price}.";
        }
    }
}
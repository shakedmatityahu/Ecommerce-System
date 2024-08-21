using MarketBackend.DAL;
namespace MarketBackend.Domain.Market_Client
{
    public class History
    {
        public SynchronizedCollection<Purchase> _purchases {get; set;}

        public History(int StoreId)
        {
            _purchases = PurchaseRepositoryRAM.GetInstance().GetShopPurchaseHistory(StoreId);
        }

        public void AddPurchase(Purchase p)
        {
                _purchases.Add(p);
                PurchaseRepositoryRAM.GetInstance().Add(p);
        }
    }
}
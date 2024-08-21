
namespace MarketBackend.Domain.Market_Client
{
    public class StoreOpenEvent : Event
    {
        private string _member;
        private Store _store;

        public StoreOpenEvent(Store store, string userName) : base("Store Open Event")
        {
            _member = userName;
            _store = store;
        }

        public override string GenerateMsg()
        {
            return $"{Name}: Member: {_member} Opened the store {_store._storeName}.";
        }
    }
}
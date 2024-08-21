
namespace MarketBackend.Domain.Market_Client
{
    public class StoreClosedEvent : Event
    {
        private string _member;
        private Store _store;

        public StoreClosedEvent(Store store, string userName) : base("Store Closed Event")
        {
            _member = userName;
            _store = store;
        }

        public override string GenerateMsg()
        {
            return $"{Name}: Member: {_member} Closed the store {_store._storeName}.";
        }
    }
}
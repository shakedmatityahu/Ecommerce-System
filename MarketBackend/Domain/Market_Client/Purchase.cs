using System.Text;
using MarketBackend.Domain.Models;
using MarketBackend.DAL.DTO;
using MarketBackend.DAL;
namespace MarketBackend.Domain.Market_Client
{
    public class Purchase{
        private int _purchaseId;
        private int _storeId;
        private string _identifier;
        private Basket _basket;
        double _price;

        public int PurchaseId { get => _purchaseId; }
        public int StoreId { get => _storeId; }
        public string Identifier { get => _identifier; }
        public double Price { get => _price; }
        public Basket Basket { get => _basket; }
        public Purchase(int id, int storeId, string identifierr, Basket basket, double basketPrice)
        {
            _purchaseId = id;
            _storeId = storeId;
            _identifier = identifierr;
            _basket = basket;
            _price = basketPrice;
        }
        public Purchase(PurchaseDTO purchaseDTO)
        {
            _purchaseId = purchaseDTO.Id;
            _storeId = purchaseDTO.StoreId;
            _identifier = purchaseDTO.Identifierr;
            try{
                _basket = BasketRepositoryRAM.GetInstance().GetById(purchaseDTO.BasketId);
            }
            catch(Exception e){
                _basket = new(purchaseDTO.BasketId, purchaseDTO.StoreId);
            }
            _price = purchaseDTO.Price;
        }

        public string GetInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("---------------------------");
            sb.AppendLine(string.Format("Purchase Number: %d", _storeId));
            sb.AppendLine(string.Format("Buyer ID: %d", _identifier));
            sb.AppendLine(string.Format("Shop ID: %d", _storeId));
            sb.AppendLine(string.Format("Basket: %s", _basket.GetInfo()));
            sb.AppendLine("---------------------------");
            return sb.ToString();
        }
    }
}
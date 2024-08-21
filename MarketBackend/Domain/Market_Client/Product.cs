using System.Collections.Concurrent;
using System.Text;
using MarketBackend.DAL;
using MarketBackend.DAL.DTO;
using Microsoft.EntityFrameworkCore;

namespace MarketBackend.Domain.Market_Client
{
    public class Product
    {
        public int _productId {get; set;}
        public int _storeId {get; set;}
        public string _name {get; set;}
        public double _price {get; set;}
        public int _quantity {get; set;}
        public string _category {get; set;}
        public ConcurrentBag<string> _keywords {get; set;}
        public string _description {get; set;}
        public ISellMethod _sellMethod {get; set;}
        public double _productRating {get; set;}
        public bool _ageLimit {get; set;}


        public int ProductId { get => _productId; }
        public int StoreId { get => _storeId; }
        public string Name { get => _name; set => _name = value; }
        public double Price { get => _price; set => _price = value; }
        public int Quantity { get => _quantity; set => _quantity = value; }
        public string Category { get => _category; set => _category = value; }
        public string Description { get => _description; set => _description = value; }
        public ConcurrentBag<string> Keywords { get => _keywords; set => _keywords = value; }
        public ISellMethod SellMethod { get => _sellMethod; set => _sellMethod = value; }
        public double ProductRating { get => _productRating; set => _productRating = value; }
        public bool AgeLimit { get => _ageLimit; }


        public Product(int id, int shopId, string name,string sellMethod, string description, double price, string category, int quantity,bool ageLimit)
        {
            _productId = id;
            _storeId = shopId;
            _name = name;
            _description = description;
            _price = price;
            _quantity = quantity;
            _category = category;
            _keywords = new ConcurrentBag<string>();
            _sellMethod = SellMethodFactory.createSellMethod(sellMethod);
            _productRating = 0;
            _ageLimit = ageLimit;
        }

        public Product(ProductDTO pdto)
        {

            _productId = pdto.ProductId;
            _description = pdto.Description;
            _name = pdto.Name;
            _price = pdto.Price;
            _quantity = pdto.Quantity;
            _category = pdto.Category;
            _keywords = [.. pdto.Keywords.Split(" ,")];
            _productRating = pdto.ProductRating;
            DBcontext context = DBcontext.GetInstance();
            List<StoreDTO> stores = context.Stores.AsNoTracking().Where(
                (s) => s.Products.Where((p) => p.ProductId == _productId).Count() > 0).ToList();
            if (pdto.ProductId == -1)
                _storeId = -1;
            else if (stores.Count() > 0)
            {
                _storeId = stores.First().Id;
            }
            else throw new Exception("Could not find shop that has this product");
            if (pdto.SellMethod == "BidSell")
            {
                _sellMethod = new BidSell();
            }
            else _sellMethod = new RegularSell();
        }


        public bool ContainKeyword(string keyWord)
        {
            return _keywords.ToList().Find((key) => key.ToLower().Equals(keyWord.ToLower())) != null;
        }

        public void AddKeyword(string keyWord)
        {
            _keywords.Add(keyWord);
            ProductRepositoryRAM.GetInstance().Update(this);
        }

        public void RemoveKeyword(string keyWord)
        {
           if(!_keywords.TryTake(out keyWord))
            {
                throw new ArgumentException("Keyword not found");
            }
            ProductRepositoryRAM.GetInstance().Update(this);
        }

        public string GetInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("---------------------------");
            sb.AppendLine(string.Format("Product ID: %d", _productId));
            sb.AppendLine(string.Format("Shop ID: %d", _storeId));
            sb.AppendLine(string.Format("Product Description: %s", _description));
            sb.AppendLine(string.Format("Quantity in stock: %d", _quantity));
            sb.AppendLine(string.Format("Catagroy: %s", _category.ToString()));
            sb.AppendLine("---------------------------");
            return sb.ToString();
        }

        public bool HasCategory(string category)
        {
            return _category == category;
        }

        public override string ToString()
        {
            return _name;
        }

        public void updatePrice(double newPrice)
        {
            if (newPrice < 0)
            {
                throw new ArgumentException("Price can't be negative");
            }
            _price = newPrice;
            ProductRepositoryRAM.GetInstance().Update(this);
        }

        public void updateQuantity(int newQuantity)
        {
            if (newQuantity < 0)
            {
                throw new ArgumentException("Quantity can't be negative");
            }
            _quantity = newQuantity;
            ProductRepositoryRAM.GetInstance().Update(this);
        }

        public Product Clone()
        {
            return new(this.ProductId, this._storeId, this._name, null, 
                this.Description, this.Price, this._category, 
                this.Quantity, this.AgeLimit){SellMethod = this.SellMethod};
        }
    }
    
}
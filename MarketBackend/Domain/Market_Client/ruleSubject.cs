using MarketBackend.DAL;
using MarketBackend.DAL.DTO;

namespace MarketBackend.Domain.Market_Client{
    public class RuleSubject{
        private Product _product;
        private string _category;
        

        public Product Product { get => _product; set => _product = value; }
        public string Category { get => _category; set => _category = value; }
        public RuleSubject() {
            _category = "All";
        }
        public RuleSubject(Product product)
        {
            _product = product;
            _category = product.Category;
        }
        public RuleSubject(string category)
        {
            _category = category;
        }
        public RuleSubject(string storeName, int storId)
        {
            _category = storeName;
        }

        //todo: nofar
        public RuleSubject(RuleSubjectDTO subject)
        {
            if (subject.Category.Equals("None"))
            {
                _category = subject.Category;
                if (subject.Product.ProductId != -1)
                {
                    _product = ProductRepositoryRAM.GetInstance().GetById(subject.Product.ProductId);
                }
                else _product = null;
            }
            else
            {
                _category = subject.Category;
            }
        }

         public RuleSubject(PolicySubjectDTO policySubject)
        {
            if (policySubject.Category.Equals("None"))
            {
                _category = policySubject.Category;
                if (policySubject.Product.ProductId != -1)
                {
                    _product = ProductRepositoryRAM.GetInstance().GetById(policySubject.Product.ProductId);
                }
                else _product = null;
            }
            else
            {
                _category = policySubject.Category;
            }
        }


        public bool IsProduct()
        {
            return _product != null;
        }
        public string GetInfo()
        {
            if (IsProduct()) { return _product?.ToString(); }
            else { return _category?.ToString(); }
        }
    }
}
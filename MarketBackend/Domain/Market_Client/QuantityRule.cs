using MarketBackend.DAL;
using MarketBackend.DAL.DTO;

namespace MarketBackend.Domain.Market_Client{
    public class QuantityRule : IConditionalRule{
        private int _minQuantity;
        private int _maxQuantity;

        public int MinQuantity { get => _minQuantity; set => _minQuantity = value; }
        public int MaxQuantity { get => _maxQuantity; set => _maxQuantity = value; }

        public QuantityRule(int id, int storeId, RuleSubject subject, int minQuantity, int maxQuantity) : base(id, storeId, subject)
        {
            _minQuantity = minQuantity;
            _maxQuantity = maxQuantity;
        }

         public QuantityRule(QuantityRuleDTO ruleDTO) : base(ruleDTO)
        {
            _minQuantity = ruleDTO.MinQuantity;
            _maxQuantity = ruleDTO.MaxQuantity;
        }

        public override bool Predicate(Basket basket)
        {
            if (Subject.IsProduct() && basket.HasProduct(Subject.Product))
            {
                return PredicateForProduct(basket);
            }
            else
            {
                return PredicateForCategory(basket);
            }
        }
        private bool PredicateForCategory(Basket basket)
        {
            int categoryCounter = 0;
            foreach (BasketItem basketItem in basket.BasketItems)
            {
                if (basketItem.Product.HasCategory(Subject.Category))
                    categoryCounter+=basketItem.Quantity;
            }
            if (categoryCounter < _minQuantity)
            {
                return false;
            }
            if (categoryCounter > _maxQuantity)
            {
                return false;
            }
            return true;
        }
        private bool PredicateForProduct(Basket basket)
        {
            BasketItem basketItem = basket.GetBasketItem(Subject.Product);
            if (basketItem.Quantity < _minQuantity)
            {
                return false;
            }
            if (basketItem.Quantity > _maxQuantity)
            {
                return false;
            }
            return true;
        }

        public override string GetInfo()
        {
            string minQuan = "NO LIMITS";
            string maxQuan = "NO LIMITS";
            if (_minQuantity != int.MinValue)
                minQuan = _minQuantity.ToString();
            if (_maxQuantity > 0)
                maxQuan = _maxQuantity.ToString();
            return $"Quantity Rule: Basket must contain at least {minQuan} and at most {maxQuan} of {Subject.GetInfo()}";
        }

        public override void Update()
        {
            RuleRepositoryRAM.GetInstance().Update(this);
        }

        public override RuleDTO CloneDTO()
        {
            return new QuantityRuleDTO(this);
        }


    }
}
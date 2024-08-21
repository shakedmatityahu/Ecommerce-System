using MarketBackend.DAL;
using MarketBackend.DAL.DTO;

namespace MarketBackend.Domain.Market_Client{
    public class TotalPriceRule : IConditionalRule{
        private double _totalPrice;
        public TotalPriceRule(int id, int shopId, RuleSubject subject, double totalPrice) : base(id, shopId, subject)
        {
            _totalPrice = totalPrice;
        }

        public TotalPriceRule(TotalPriceRuleDTO ruleDTO) : base(ruleDTO)
        {
            _totalPrice = ruleDTO.TotalPrice;
        }

        public double TotalPrice { get => _totalPrice; set => _totalPrice = value; }

        public override string GetInfo()
        {
            return $"Total Price Rule: Basket price must be at least {_totalPrice}";
        }

        public override bool Predicate(Basket basket)
        {
            return basket.GetBasketPriceBeforeDiscounts() > _totalPrice;
        }

        public override void Update()
        {
            RuleRepositoryRAM.GetInstance().Update(this);
        }

        public override RuleDTO CloneDTO()
        {
            return new TotalPriceRuleDTO(this);
        }
    }
}
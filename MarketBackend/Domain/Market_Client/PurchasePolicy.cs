using MarketBackend.DAL.DTO;

namespace MarketBackend.Domain.Market_Client
{
     public class PurchasePolicy : IPolicy{
        public PurchasePolicy(int id,int storeId, DateTime expirationDate, RuleSubject subject, IRule rule) : base(id,storeId, expirationDate, subject, rule)
        {
        }
        public PurchasePolicy(PurchasePolicyDTO purchasePolicyDTO) : base(purchasePolicyDTO)
        {
        }
        public override void Apply(Basket basket)
        {
            if (!IsExpired())
            {
                if (!IsValidForBasket(basket))
                    throw new Exception("Basket does not stand with purchase policy constraints");
            }
        }
        public override string GetInfo()
        {
            return $"Purchase Policy on {Rule.Subject.GetInfo()}\nRule - {Rule.GetInfo()}";
        }

        public override bool IsValidForBasket(Basket basket)
        {
            return Rule.Predicate(basket);
        }

        public override PurchasePolicyDTO CloneDTO()
        {
            return new PurchasePolicyDTO(this);
        }
    }

}
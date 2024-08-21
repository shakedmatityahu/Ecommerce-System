using MarketBackend.DAL;
using MarketBackend.DAL.DTO;

namespace MarketBackend.Domain.Market_Client
{
    public class SimpleRule : IRule
    {
        public SimpleRule(int id, int shopId, RuleSubject subject) : base(id, shopId)
        {
            Subject = subject;
        }

        public SimpleRule(SimpleRuleDTO ruleDTO):base(ruleDTO)
        {
            
        }

         public override bool Predicate(Basket basket)
        {
            if (Subject.IsProduct())
            {
                return basket.HasProduct(Subject.Product);
            }
            else
            {
                foreach (BasketItem basketItem in basket.BasketItems)
                {
                    if (basketItem.Product.HasCategory(Subject.Category))
                        return true;
                }
            }
            return false;
        }
        
        public override void Update()
        {
            RuleRepositoryRAM.GetInstance().Update(this);
        }
        
        public override string GetInfo()
        {
            return $"Simple Rule: Basket must contain at least one {Subject.GetInfo()}";
        }

        public override RuleDTO CloneDTO()
        {
            return new SimpleRuleDTO(this);
        }

    }
}
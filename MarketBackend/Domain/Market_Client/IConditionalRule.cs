using MarketBackend.DAL.DTO;

namespace MarketBackend.Domain.Market_Client{
    public abstract class IConditionalRule : IRule{
        public IConditionalRule(int id, int shopId, RuleSubject subject) : base(id, shopId)
        {
            Subject = subject;
        }

        public IConditionalRule(RuleDTO ruleDTO) : base(ruleDTO)
        {
        }

        public abstract override string GetInfo();

        public abstract override bool Predicate(Basket basket);
    }
}
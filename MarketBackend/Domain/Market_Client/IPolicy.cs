using MarketBackend.DAL;
using MarketBackend.DAL.DTO;
using Microsoft.EntityFrameworkCore;



namespace MarketBackend.Domain.Market_Client{
 public abstract class IPolicy
    {
        private int _id;
        private DateTime _expirationDate;
        private int _storeId;
        private IRule _rule;
        private RuleSubject _subject;

        public int Id { get => _id; set => _id = value; }
        public DateTime ExpirationDate { get => _expirationDate; set => _expirationDate = value; }
        public int StoreId { get => _storeId; set => _storeId = value; }
        public IRule Rule { get => _rule; set => _rule = value; }
        public RuleSubject Subject { get => _subject; set => _subject = value; }

        public IPolicy(int id, int storeId, DateTime expirationDate, RuleSubject subject, IRule rule)
        {
            _id = id;
            _rule = rule;
            _expirationDate = expirationDate;
            _subject = subject;
            _storeId = storeId;
        }

        public IPolicy(int id ,int storeId, DateTime expirationDate, RuleSubject subject)
        {
            _id = id;
            _expirationDate = expirationDate;
            _subject = subject;
            _storeId = storeId;
        }

        protected IPolicy(DiscountPolicyDTO discountPolicyDTO)
        {
            _id = discountPolicyDTO.Id;
            _expirationDate = discountPolicyDTO.ExpirationDate;
            _subject = new RuleSubject(discountPolicyDTO.PolicySubject);
            _rule = RuleRepositoryRAM.GetInstance().GetById(discountPolicyDTO.RuleId);
            if (_rule.Id == -1) _rule = null;
            _storeId = DBcontext.GetInstance().Stores
                .Include(s => s.Rules)
                .FirstOrDefault(s => s.Policies.Any(policy => policy.Id == policy.Id))
                .Id;
        }
        protected IPolicy(PurchasePolicyDTO purchasePolicyDTO)
        {
            _id = purchasePolicyDTO.Id;
            _expirationDate = purchasePolicyDTO.ExpirationDate;
            _subject = new RuleSubject(purchasePolicyDTO.PolicySubject);
            _rule = RuleRepositoryRAM.GetInstance().GetById(purchasePolicyDTO.RuleId);
            _storeId = DBcontext.GetInstance().Stores
                .Include(s => s.Rules)
                .FirstOrDefault(s => s.Policies.Any(policy => policy.Id == policy.Id))
                .Id;
        }


        public abstract void Apply(Basket basket);
        public abstract string GetInfo();
        public abstract bool IsValidForBasket(Basket basket);
        public bool IsExpired()
        {
            return _expirationDate < DateTime.Now;
        }

        public abstract PolicyDTO CloneDTO();

    }
}

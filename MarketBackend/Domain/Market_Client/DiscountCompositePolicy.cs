using MarketBackend.DAL;
using MarketBackend.DAL.DTO;
namespace MarketBackend.Domain.Market_Client
{
    
    public class DiscountCompositePolicy : DiscountPolicy
    {
        private NumericOperator _numericOperator;
        private List<IPolicy> _policies;
        public NumericOperator NumericOperator { get => _numericOperator; set => _numericOperator = value; }
        public List<IPolicy> Policies { get => _policies; set => _policies = value; }

        public DiscountCompositePolicy(int id,int storeId, DateTime expirationDate, RuleSubject subject, NumericOperator Operator, List<IPolicy> policies) :base(id, storeId, expirationDate, subject)
        {
            Precentage = 0;
            Subject = new RuleSubject();
            _numericOperator = Operator;
            _policies = policies;
            Rule = GenerateDummyRule(); //dummy rule
        }

        public DiscountCompositePolicy(DiscountCompositePolicyDTO discountCompositePolicyDTO, List<IPolicy> policies) : base(discountCompositePolicyDTO)
        {
            Precentage = 0;
            Subject = new RuleSubject();
            _numericOperator = CastOperator(discountCompositePolicyDTO.NumericOperator);
            _policies = policies;

        }

        private NumericOperator CastOperator(string operatorName)
        {
            try
            {
                return (NumericOperator)Enum.Parse(typeof(NumericOperator), operatorName);
            }
            catch { throw new Exception("Invalid operator name"); }
        }
        public IRule GenerateDummyRule()
        {
            if (RuleRepositoryRAM.GetInstance().ContainsID(-1))
                return RuleRepositoryRAM.GetInstance().GetById(-1);
            return new SimpleRule(-1, -1, new RuleSubject());
        }

        public override void Apply(Basket basket)
        {
            switch (_numericOperator)
            {
                case NumericOperator.Add: ApplyAdd(basket); break;
                case NumericOperator.Max: ApplyMax(basket); break;
            }
        }

         protected void ApplyAdd(Basket basket)
        {
            foreach (IPolicy policy in Policies)
                policy.Apply(basket);
        }

        protected void ApplyMax(Basket basket)
        {
            IPolicy maxProfitPolicy = GetMaxProfitPolicy(basket);
            maxProfitPolicy.Apply(basket);
        }
        private DiscountPolicy GetMaxProfitPolicy(Basket basket)
        {
            int bestDiscountIndex = 0;
            double bestDiscount = 0;
            for (int i = 0; i < Policies.Count; i++)
            {
                double currDiscount = ((DiscountPolicy)Policies[i]).GetDiscount(basket);
                if (currDiscount > bestDiscount)
                {
                    bestDiscount = currDiscount;
                    bestDiscountIndex = i;
                }
            }
            return (DiscountPolicy)Policies[bestDiscountIndex];
        }
        public override DiscountCompositePolicyDTO CloneDTO()
        {
            return new DiscountCompositePolicyDTO(this);
        }

    }
}
namespace MarketBackend.Domain.Market_Client
{
    public class StoreRuleFactory{
        private int _storeId;
        RuleBuilder ruleBuilder;
        public StoreRuleFactory(int storeId)
        {
            _storeId = storeId;
            ruleBuilder = new RuleBuilder(storeId);
        }
        public StoreRuleFactory(int storeId,int ruleIdFactory)
        {
            _storeId = storeId;
            ruleBuilder = new RuleBuilder(storeId, ruleIdFactory);
        }

        public IRule makeRule(Type type)
        {
            ruleBuilder.BuildID();
            switch (type.Name)
            {
                case "SimpleRule": ruleBuilder.makeSimpleRule(); break;
                case "QuantityRule": ruleBuilder.makeQuantityRule(); break;
                case "TotalPriceRule": ruleBuilder.makeTotalPriceRule(); break;
                case "CompositeRule": ruleBuilder.makeCompositeRule(); break;
            }
            return ruleBuilder.Build();
        }

        public void setFeatures(RuleSubject productOrCategory)
        {
            ruleBuilder.reset();
            ruleBuilder.buildFeatures(productOrCategory);
        }

        public void setFeatures(RuleSubject productOrCategory, int minQuantity, int maxQuantity)
        {
            ruleBuilder.reset();
            ruleBuilder.buildFeatures(productOrCategory, minQuantity, maxQuantity);
        }

        public void setFeatures(RuleSubject productOrCategory, int targetPrice)
        {
            ruleBuilder.reset();
            ruleBuilder.buildFeatures(productOrCategory, targetPrice);        }

        public void setFeatures(LogicalOperator Operator, List<IRule> rules)
        {
            ruleBuilder.reset();
            ruleBuilder.buildFeatures(Operator, rules);
        }
    }
}

using MarketBackend.DAL;

namespace MarketBackend.Domain.Market_Client
{
    public class DiscountPolicyManager : IPolicyManager<DiscountPolicy>
    {
        public DiscountPolicyManager(int storeId):base(storeId)
        {
        }

        public override DiscountPolicy GetPolicy(int policyId)
        {
            if (!Policies.TryGetValue(policyId, out IPolicy policy))
            {
                if (PolicyRepositoryRAM.GetInstance().ContainsID(policyId))
                    return (DiscountPolicy)PolicyRepositoryRAM.GetInstance().GetById(policyId);
                throw new Exception("Policy was not found");
            }
            return (DiscountPolicy)policy;
        }

        public int AddPolicy(int id, DateTime expirationDate, RuleSubject subject, IRule rule, double precentage)
        {
            int unicId = int.Parse($"{_storeId}{id}");
            DiscountPolicy policy = new DiscountPolicy(unicId, StoreId, expirationDate, subject, rule, precentage);
            Policies.TryAdd(policy.Id, policy);
            PolicyRepositoryRAM.GetInstance().Add(policy);
            return policy.Id;
        }

        public int AddCompositePolicy(int id, DateTime expirationDate, RuleSubject subject, NumericOperator Operator, List<int> policies)
        {
            List<IPolicy> policiesToAdd = new List<IPolicy>();
            foreach(int policyId in policies)
            {
                IPolicy p = GetPolicy(policyId) ?? throw new Exception("Policy was not found");
            }
            foreach (int policyId in policies)
            {
                policiesToAdd.Add(GetPolicy(policyId));
                Policies.TryRemove(policyId, out IPolicy dummy);
                PolicyRepositoryRAM.GetInstance().Delete(policyId);
            }
            int unicId = int.Parse($"{_storeId}{id}");
            DiscountCompositePolicy policy = new DiscountCompositePolicy(unicId,_storeId, expirationDate, subject, Operator, policiesToAdd);
            Policies.TryAdd(policy.Id, policy);
            PolicyRepositoryRAM.GetInstance().Add(policy);
            return policy.Id;
        }

    }
}
using MarketBackend.DAL;

namespace MarketBackend.Domain.Market_Client
{
  public class PurchasePolicyManager : IPolicyManager<PurchasePolicy>
    {
        public PurchasePolicyManager(int storeId):base(storeId)
        {
        }

        public int AddPolicy(int id, DateTime expirationDate,RuleSubject subject, IRule rule)
        {
            int unicId = int.Parse($"{_storeId}{id}");
            PurchasePolicy policy = new PurchasePolicy(unicId, StoreId, expirationDate, subject, rule);
            Policies.TryAdd(policy.Id, policy);
            PolicyRepositoryRAM.GetInstance().Add(policy);
            return policy.Id;
        }
        public override PurchasePolicy GetPolicy(int policyId)
        {
            if (!Policies.TryGetValue(policyId, out IPolicy policy))
            {
                if (PolicyRepositoryRAM.GetInstance().ContainsID(policyId))
                    return (PurchasePolicy)PolicyRepositoryRAM.GetInstance().GetById(policyId);
                throw new Exception("Policy was not found");
            }
            return (PurchasePolicy)policy;
        }
    }
}
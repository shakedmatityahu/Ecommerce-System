using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketBackend.DAL;

namespace MarketBackend.Domain.Market_Client
{
    public abstract class IPolicyManager<T> where T : IPolicy
    {
        protected int _storeId;
        private ConcurrentDictionary<int, IPolicy> _policies;
        public ConcurrentDictionary<int, IPolicy> Policies { get => _policies; set => _policies = value; }

        protected IPolicyManager(int storeId)
        {
            StoreId = storeId;
            _policies = new ConcurrentDictionary<int, IPolicy>();
        }

        public int StoreId { get => _storeId; set => _storeId = value; }

        public abstract T GetPolicy(int policyId);
        public void RemovePolicy(int policyId)
        {
            if (!_policies.TryRemove(policyId, out IPolicy removed))
            {
                throw new Exception("Policy was not found");
            }
            PolicyRepositoryRAM.GetInstance().Delete(policyId);
        }
        public void Apply(Basket basket)
        {
            CleanExpiredPolicies();
            IPolicy[] policies = _policies.Values.ToArray();
            foreach (IPolicy policy in policies)
            {
                policy.Apply(basket);
            }
        }
        public void CleanExpiredPolicies()
        {
            List<IPolicy> policies = _policies.Values.ToList();
            foreach (IPolicy policy in policies)
            {
                if (policy.IsExpired())
                {
                    _policies.TryRemove(policy.Id, out _);
                }
            }
        }
    }
}
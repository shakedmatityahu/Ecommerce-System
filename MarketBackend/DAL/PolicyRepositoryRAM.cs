using MarketBackend.Domain.Market_Client;
using System.Collections.Concurrent;
using MarketBackend.Services.Interfaces;
using MarketBackend.DAL.DTO;


namespace MarketBackend.DAL
{
    public class PolicyRepositoryRAM : IPolicyRepository {
        private static ConcurrentDictionary<int, IPolicy> _policyById;

        private static PolicyRepositoryRAM _policyRepo = null;
        private object Lock;

        private PolicyRepositoryRAM()
        {
            _policyById = new ConcurrentDictionary<int, IPolicy>();
            Lock = new object();
        }
        public static PolicyRepositoryRAM GetInstance()
        {
            if (_policyRepo == null)
                _policyRepo = new PolicyRepositoryRAM();
            return _policyRepo;
        }

        public void Add(IPolicy policy)
        {
            try{
                _policyById.TryAdd(policy.Id, policy);
                lock(Lock){
                    DBcontext.GetInstance().Stores.Find(policy.StoreId).Policies.Add(policy.CloneDTO());
                    DBcontext.GetInstance().SaveChanges();
                }
            }
            catch(Exception){
                throw new Exception("There was a problem in Database use- Add Policy");
            }
            
        }

        public bool ContainsID(int id)
        {
            try{
                if (_policyById.ContainsKey(id))
                return true;
                else return DBcontext.GetInstance().Stores.Any(s => s.Rules.Any(r => r.Id.Equals(id)));
            }
            catch(Exception){
                throw new Exception("There was a problem in Database use- Contains Policy");
            }
        }

         public IPolicy GetById(int id)
        {
            if (_policyById.ContainsKey(id))
                return _policyById[id];
            else if (ContainsID(id))
            {
                try{
                    lock(Lock){
                    DBcontext context = DBcontext.GetInstance();
                    StoreDTO shopDto = context.Stores.Where(s => s.Policies.Any(p => p.Id == id)).FirstOrDefault();
                    PolicyDTO policyDTO = shopDto.Policies.Find(p => p.Id == id);
                    _policyById.TryAdd(id, makePolicy(policyDTO));
                    return _policyById[id];
                    }
                }
                catch(Exception){
                throw new Exception("There was a problem in Database use- Get Policy");
                }
            }
            else
                throw new ArgumentException("Invalid Rule Id.");
        }

        public IEnumerable<IPolicy> getAll()
        {
            UploadRulesFromContext();
            return _policyById.Values.ToList();
        }

        
        public void Delete(int id)
        {
            if (_policyById.ContainsKey(id))
            {
                try{
                    lock(Lock){
                    StoreDTO store = DBcontext.GetInstance().Stores.Find(_policyById[id].StoreId);
                    _policyById.TryRemove(id, out IPolicy removed);
                    PolicyDTO p = store.Policies.Find(p => p.Id == id);
                    store.Policies.Remove(p);
                    DBcontext.GetInstance().Policies.Remove(p);
                    DBcontext.GetInstance().SaveChanges();
                    }
                }
                catch(Exception){
                throw new Exception("There was a problem in Database use- Delete Policy");
                }
            }
            else throw new Exception("Product Id does not exist."); ;
        }

        public void Update(IPolicy policy)
        {
            if (_policyById.ContainsKey(policy.Id)){
                _policyById[policy.Id] = policy;
                try{
                    lock(Lock){
                    PolicyDTO p = DBcontext.GetInstance().Policies.Find(policy.Id);
                    if (p != null){
                        p.ExpirationDate = policy.ExpirationDate;
                        p.RuleId = policy.Rule.Id;
                        p.PolicySubject = new PolicySubjectDTO(policy.Subject);
                    }
                    DBcontext.GetInstance().SaveChanges();
                    }
                }
                catch(Exception){
                throw new Exception("There was a problem in Database use- Update Policy");
                }
            }
            else{
                throw new KeyNotFoundException($"Policy with ID {policy.Id} not found.");
            }

        }

        private void UploadRulesFromContext()
        {
            List<StoreDTO> stores = DBcontext.GetInstance().Stores.ToList();
            foreach (StoreDTO storeDTO in stores)
            {
                UploadShopPoliciesFromContext(storeDTO.Id);
            }
        }

        private void UploadShopPoliciesFromContext(int storeId)
        {
            try{
                lock(Lock){
                    StoreDTO storeDto = DBcontext.GetInstance().Stores.Find(storeId);
                    if (storeDto != null)
                    {
                        if (storeDto.Rules != null)
                        {
                            List<PolicyDTO> policies = storeDto.Policies.ToList();
                            foreach (PolicyDTO policyDTO in policies)
                            {
                                _policyById.TryAdd(policyDTO.Id, makePolicy(policyDTO));
                            }
                        }
                    }
                }
            }
            catch(Exception){
                throw new Exception("There was a problem in Database use- Upload Policy");
            }
            
        }

        public IPolicy makePolicy(PolicyDTO policyDTO)
        {
            Type policyType = policyDTO.GetType();
            if (policyType.Name.Equals("DiscountPolicyDTO"))
            {
                return new DiscountPolicy((DiscountPolicyDTO)policyDTO);
            }
            else if (policyType.Name.Equals("PurchasePolicyDTO"))
            {
                return new PurchasePolicy((PurchasePolicyDTO)policyDTO);
            }
            else if (policyType.Name.Equals("DiscountCompositePolicyDTO"))
            {
                List<IPolicy> policies = new List<IPolicy>();
                foreach (PolicyDTO p in ((DiscountCompositePolicyDTO)policyDTO).Policies)
                {
                    policies.Add(makePolicy(p));
                }
                return new DiscountCompositePolicy((DiscountCompositePolicyDTO)policyDTO, policies);
            }
            return null;
        }
        public void Clear()
        {
            _policyById.Clear();
        }
        public void ResetDomainData()
        {
            _policyById = new ConcurrentDictionary<int, IPolicy>();
        }

        public void Delete(IPolicy policy)
        {
            Delete(policy.Id);
        }

        public static void Dispose()
        {
             _policyById = new ConcurrentDictionary<int, IPolicy>();
        }
    }
}
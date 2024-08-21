using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketBackend.Domain.Models;
using MarketBackend.DAL;
using MarketBackend.Domain.Payment;
using MarketBackend.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Security.Policy;
using System.Data;
using MarketBackend.Domain.Shipping;
using Microsoft.IdentityModel.Tokens;
using MarketBackend.DAL.DTO;


namespace MarketBackend.Domain.Market_Client
{
    public class MarketManagerFacade : IMarketManagerFacade
    {
        private static MarketManagerFacade marketManagerFacade = null;
        private readonly IStoreRepository _storeRepository;
        private readonly ClientManager _clientManager;
        private readonly IPaymentSystemFacade _paymentSystem;
        private readonly IShippingSystemFacade _shippingSystemFacade;
        private int _storeCounter = 1;

        
        private MarketManagerFacade(IShippingSystemFacade shippingSystemFacade, IPaymentSystemFacade paymentSystem){
            _storeRepository = StoreRepositoryRAM.GetInstance();
            _clientManager = ClientManager.GetInstance();
            _storeCounter = UpdateStoreCounter();
            _paymentSystem = paymentSystem;
            _shippingSystemFacade = shippingSystemFacade;
            _shippingSystemFacade.Connect();
            _paymentSystem.Connect();
            // InitiateSystemAdmin();
            
        }

        public static MarketManagerFacade GetInstance(IShippingSystemFacade shippingSystemFacade, IPaymentSystemFacade paymentSystem){
            if (marketManagerFacade == null){
                marketManagerFacade = new MarketManagerFacade(shippingSystemFacade, paymentSystem);
            }
            return marketManagerFacade;
        }

        public static void Dispose(){
            StoreRepositoryRAM.Dispose();
            BasketRepositoryRAM.Dispose();
            ClientRepositoryRAM.Dispose();
            ProductRepositoryRAM.Dispose();
            RoleRepositoryRAM.Dispose();
            StoreRepositoryRAM.Dispose();
            ClientManager.Dispose();
            PurchaseRepositoryRAM.Dispose();
            marketManagerFacade = null;            
        }
        
        public void InitiateSystemAdmin()
        {
            _clientManager.RegisterAsSystemAdmin("system_admin", "system_admin", "system.admin@mail.com", 30);            
        }
        public void RegisterAsSystemAdmin(string username, string password, string email, int age)
        {
            _clientManager.RegisterAsSystemAdmin(username, password, email, age);
        }
        public void AddManger(string identifier, int storeId, string toAddUserName)
        {
            Store store = _storeRepository.GetById(storeId);
            if (store != null && _clientManager.CheckMemberIsLoggedIn(identifier))
            {
                Member activeMember = (Member)_clientManager.GetClientByIdentifier(identifier);
                Role role = new Role(new StoreManagerRole(RoleName.Manager), activeMember, storeId, toAddUserName);
                store.AddStaffMember(toAddUserName, role, activeMember.UserName);
            }
            else
                throw new Exception("Store doesn't exist!");

        }

        public void AddOwner(string identifier, int storeId, string userName)
        {
            Store store = _storeRepository.GetById(storeId);
            if (store != null && _clientManager.CheckMemberIsLoggedIn(identifier))
            {
                Member activeMember = (Member)_clientManager.GetClientByIdentifier(identifier);
                Role role = new Role(new Owner(RoleName.Owner), activeMember, storeId, userName);
                store.AddStaffMember(userName, role, activeMember.UserName);
            }
            else
                throw new Exception("Store doesn't exist!");

        }

        public void AddPermission(string identifier, int storeId, string toAddUserName, Permission permission)
        {
            Store store = _storeRepository.GetById(storeId);
            if (store != null && _clientManager.CheckMemberIsLoggedIn(identifier))
            {
                Member activeMember = (Member)_clientManager.GetClientByIdentifier(identifier);
                store.AddPermission(activeMember.UserName, toAddUserName, permission);
            }
            else
                throw new Exception("Store doesn't exist!");
        }

        public void RemovePermission(string identifier, int storeId, string toRemoveUserName, Permission permission)
        {
            Store store = _storeRepository.GetById(storeId);
            if (store != null && _clientManager.CheckMemberIsLoggedIn(identifier))
            {
                Member activeMember = (Member)_clientManager.GetClientByIdentifier(identifier);
                store.RemovePermission(activeMember.UserName, toRemoveUserName, permission);
            }
            else
                throw new Exception("Store doesn't exist!");

        }


        public Product AddProduct(int storeId, string identifier, string name, string sellMethod, string description, double price, string category, int quantity, bool ageLimit)
        {
            Store store = _storeRepository.GetById(storeId);
            if (store != null && _clientManager.CheckMemberIsLoggedIn(identifier))
            {
                Member activeMember = (Member)_clientManager.GetClientByIdentifier(identifier);
                return store.AddProduct(activeMember.UserName, name, sellMethod, description, price, category, quantity, ageLimit);
            }
            else
                throw new Exception("Store doesn't exist!");

        }

        public void AddToCart(string identifier, int storeId, int productId, int quantity)
        {
            ClientManager.CheckClientIdentifier(identifier);
            Store store = _storeRepository.GetById(storeId);
            bool found = false;
            if (store != null){
                foreach (var product in store._products){
                    if (product._productId == productId){
                        found = true;
                        break;
                    }
                }
                if (found){
                    using var transaction = DBcontext.GetInstance().Database.BeginTransaction();
                    try{
                        _clientManager.AddToCart(identifier, storeId, productId, quantity);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception(ex.Message);
                    }
                } 
                else
                    throw new Exception($"No productid {productId}");
            }
            else
                throw new Exception($"Store {store} doesn't exists.");
            
        }

        public void CloseStore(string identifier, int storeId)
        {
            Store store = _storeRepository.GetById(storeId);
            if (store != null && _clientManager.CheckMemberIsLoggedIn(identifier)){
                Member activeMember = (Member)_clientManager.GetClientByIdentifier(identifier);
                store.CloseStore(activeMember.UserName);
                StoreRepositoryRAM.GetInstance().Update(store);
            }
            else{
                throw new Exception("Store doesn't exists");
            }
        }

    
        public int CreateStore(string identifier, string storeName, string email, string phoneNum)
        {
            int storeId=-1;
            Client store_founder = _clientManager.GetClientByIdentifier(identifier);
            if(store_founder != null && _clientManager.CheckMemberIsLoggedIn(identifier))
            {
                using var transaction = DBcontext.GetInstance().Database.BeginTransaction();
                try{
                    storeId = _storeCounter++;
                    if (_storeRepository.GetById(storeId) != null){
                        throw new Exception("Store exists");
                    }
                    Store store = new Store(storeId, storeName, email, phoneNum)
                    {
                        _active = true
                    };
                    _storeRepository.Add(store);
                    Member activeMember = (Member)_clientManager.GetClientByIdentifier(identifier);
                    Role role = new Role(new Founder(RoleName.Founder), activeMember, storeId, activeMember.UserName);

                    // store.SubscribeStoreOwner(activeMember);
                    store.AddStaffMember(activeMember.UserName, role, activeMember.UserName); //adds himself
                    transaction.Commit();
                }
                catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception(ex.Message);
                }
            }
            else
            {
                throw new Exception("Store founder must be a Member.");
            }
            return storeId;
        }

        public void EditPurchasePolicy(int storeId)
        {
            throw new NotImplementedException();
        }

        public void EnterAsGuest(string identifier)
        {
            _clientManager.BrowseAsGuest(identifier);
        }

        public void ExitGuest(string identifier)
        {
            _clientManager.DeactivateGuest(identifier);
        }

        public Member GetFounder(int storeId)
        {
            Store store = _storeRepository.GetById(storeId);
            if (store != null)
            {
                string founderUsername = store.roles.FirstOrDefault(pair => pair.Value.getRoleName() == RoleName.Founder).Key;
                if (_clientManager.IsMember(founderUsername))
                    return _clientManager.GetMemberByUserName(founderUsername);
                else
                    throw new Exception("should not happen! founder is not a member");
            }
            else
                throw new Exception("Store doesn't exist!");
        }

        public List<Member> GetMangers(int storeId)
        {
            Store store = _storeRepository.GetById(storeId);
            if (store != null)
            {

                List<string> usernames = store.roles.Where(pair => pair.Value.getRoleName() == RoleName.Manager).Select(pair => pair.Key).ToList();
                List<Member> managers = new();
                usernames.ForEach(username => managers.Add((Member)_clientManager.GetMemberByUserName(username)));
                return managers;
            }
            else
                throw new Exception("Store doesn't exist!");

        }

        public List<Member> GetOwners(int storeId)
        {
            Store store = _storeRepository.GetById(storeId);
            if (store != null)
            {

                List<string> usernames = store.roles.Where(pair => pair.Value.getRoleName() == RoleName.Owner).Select(pair => pair.Key).ToList();
                List<Member> managers = new List<Member>();
                usernames.ForEach(useerName => managers.Add((Member)_clientManager.GetMemberByUserName(useerName)));
                return managers;
            }
            else
                throw new Exception("Store doesn't exist!");
        }

        public string GetProductInfo(int storeId, int productId)
        {
            Store store = _storeRepository.GetById(storeId);
            if (store == null){
                throw new Exception("Store doesn't exists");
            }
            return store.getProductInfo(productId);
        }

        public Product GetProduct(int storeId, int productId)
        {
            Store store = _storeRepository.GetById(storeId);
            if (store == null){
                throw new Exception("Store doesn't exists");
            }
            return store.GetProduct(productId);
        }

        public string GetInfo(int storeId){
            Store store = _storeRepository.GetById(storeId);
            if (store == null){
                throw new Exception("Store doesn't exists");
            }
            return store.getInfo();
        }

        public List<ShoppingCartHistory> GetPurchaseHistoryByClient(string userName)
        {
            return _clientManager.GetPurchaseHistoryByClient(userName);
        }

        public List<Purchase> GetPurchaseHistoryByStore(int storeId, string identifier)
        {
            Store store = _storeRepository.GetById(storeId);
            if (store != null){           
                var member  = _clientManager.GetMemberByIdentifier(identifier);
                return store.getHistory(member.UserName);
            }
            else{
                throw new Exception("Store doesn't exists");
            }
        }

        public bool IsAvailable(int storeId)
        {
            Store store = _storeRepository.GetById(storeId);
            if (store != null){
                return store._active;
            }
            else{
                throw new Exception("Store doesn't exists");
            }
        }

        public string LoginClient(string username, string password)
        {
            return _clientManager.LoginClient(username, password);
        }

        public void LogoutClient(string identifier)
        {
            _clientManager.LogoutClient(identifier);
        }

        public void OpenStore(string identifier, int storeId)
        {
            Store store = _storeRepository.GetById(storeId);
            if (store != null && _clientManager.CheckMemberIsLoggedIn(identifier)){
                Member activeMember = (Member)_clientManager.GetClientByIdentifier(identifier);
                store.OpenStore(activeMember.UserName);
                StoreRepositoryRAM.GetInstance().Update(store);
            }
            else{
                throw new Exception("Store doesn't exists");
            }
        }

        public void PurchaseCart(string identifier, PaymentDetails paymentDetails, ShippingDetails shippingDetails) //clientId
        {
            using var transaction = DBcontext.GetInstance().Database.BeginTransaction();
            try{
                ClientManager.CheckClientIdentifier(identifier);
                var client = _clientManager.GetClientByIdentifier(identifier);
                var baskets = client.Cart.GetBaskets();
                if (baskets.IsNullOrEmpty()){
                    throw new Exception("Empty cart.");
                }
                var stores = new List<Store>();
                foreach(var basket in baskets){
                    var store = _storeRepository.GetById(basket.Key);
                    stores.Add(store);
                    if(!store.checkBasketInSupply(basket.Value)) throw new Exception("unavailable."); 
                    if(!store.checklegalBasket(basket.Value, client.IsAbove18)) throw new Exception("unavailable.");               
                }
                foreach(var store in stores){
                    var totalPrice = store.CalculateBasketPrice(baskets[store.StoreId]);
                    if(_paymentSystem.Pay(paymentDetails, totalPrice) > 0) {
                        if(_shippingSystemFacade.OrderShippment(shippingDetails) > 0){
                            store.PurchaseBasket(identifier, baskets[store.StoreId]);
                            _clientManager.PurchaseBasket(identifier, baskets[store.StoreId]);
                        }
                        else{
                            throw new Exception("shippment failed.");
                        }                  
                    }
                    else 
                        throw new Exception("payment failed.");
                }
                transaction.Commit();
            }
            catch(Exception e) 
            {
                transaction.Rollback();
                throw new Exception(e.Message); 
            }           

        }

        public void Register(string username, string password, string email, int age)
        {
            _clientManager.Register(username, password, email, age);
        }

        public void RemoveFromCart(string identifier, int productId, int storeId, int quantity)
        {
            _clientManager.RemoveFromCart(identifier, productId, storeId, quantity);
        }

        public void RemoveManger(string identifier, int storeId, string toRemoveUserName)
        {
            RemoveStaffMember(storeId, identifier, toRemoveUserName);
        }

        public void RemoveOwner(string identifier, int storeId, string toRemoveUserName)
        {
            RemoveStaffMember(storeId, identifier, toRemoveUserName);
        }

        public void RemoveProduct(int storeId, string identifier, int productId)
        {
            Store store = _storeRepository.GetById(storeId);
            if (store == null){
                throw new Exception("Store doesn't exists");
            }
            Member activeMember = (Member)_clientManager.GetClientByIdentifier(identifier);
            store.RemoveProduct(activeMember.UserName, productId);
        }

        public void RemoveStaffMember(int storeId, string identifier, string toRemoveUserName)
        {
            Store store = _storeRepository.GetById(storeId);
            if (store != null)
            {
                Member activeMember = (Member)_clientManager.GetClientByIdentifier(identifier);
                store.RemoveStaffMember(toRemoveUserName, activeMember.UserName);
                //need to do also in the db
            }
            else
                throw new Exception("Store doesn't exist!");
        }

        public bool ResToStoreManageReq(string identifier)
        {
            throw new NotImplementedException();
        }

        public bool ResToStoreOwnershipReq(string identifier)
        {
            throw new NotImplementedException();
        }

        public HashSet<Product> SearchByCategory(string category)
        {
            return SearchingManager.searchByCategory(category);
        }

        public HashSet<Product> SearchByKeyWords(string keywords)
        {
            return SearchingManager.searchByKeyword(keywords);
        }

        public HashSet<Product> SearchByName(string name)
        {
            return SearchingManager.serachByName(name);
        }
        public HashSet<Product> SearchByCategoryWithStore(int storeId, string category)
        {
            return SearchingManager.searchByCategoryWithStore(storeId, category);
        }

        public HashSet<Product> SearchByKeyWordsWithStore(int storeId, string keywords)
        {
            return SearchingManager.searchByKeywordWithStore(storeId, keywords);
        }

        public HashSet<Product> SearchByNameWithStore(int storeId, string name)
        {
            return SearchingManager.serachByNameWithStore(storeId, name);
        }

        public void Filter(HashSet<Product> products, string category, double lowPrice, double highPrice, double lowProductRate, double highProductRate, double lowStoreRate, double highStoreRate)
        {
            FilterParameterManager filter = new FilterParameterManager(category, lowPrice, highPrice, lowProductRate, highProductRate, lowStoreRate, highStoreRate);
            filter.Filter(products);
        }

        public void UpdateProductPrice(int storeId, string identifier,  int productId, double price)
        {
            if (_storeRepository.GetById(storeId) != null && _clientManager.CheckMemberIsLoggedIn(identifier))
            {
                Member activeMember = (Member)_clientManager.GetClientByIdentifier(identifier);
                _storeRepository.GetById(storeId).UpdateProductPrice(activeMember.UserName, productId, price);
            }
            else
            {
                throw new Exception("Store not found");
            }

        }

        public void UpdateProductQuantity(int storeId, string identifier, int productId, int quantity)
        {
            if (_storeRepository.GetById(storeId) != null && _clientManager.CheckMemberIsLoggedIn(identifier)) 
            {
                Member activeMember = (Member)_clientManager.GetClientByIdentifier(identifier);
                _storeRepository.GetById(storeId).UpdateProductQuantity(activeMember.UserName, productId, quantity);
            }
            else
            {
                throw new Exception("Store not found");
            }
        }

        public ShoppingCart ViewCart(string identifier)
        {
            ClientManager.CheckClientIdentifier(identifier);
            return _clientManager.ViewCart(identifier);
        }

        public void AddStaffMember(int storeId, string identifier, string roleName, string toAddUserName){            
            Store store = _storeRepository.GetById(storeId);
            if (store != null && _clientManager.CheckMemberIsLoggedIn(identifier))
            {
                using var transaction = DBcontext.GetInstance().Database.BeginTransaction();
                try{
                    Member appoint = _clientManager.GetMemberByIdentifier(identifier);
                    Member appointe = _clientManager.GetMemberByUserName(toAddUserName);
                    RoleType roleType = RoleType.GetRoleTypeFromDescription(roleName);
                    Role role = new(roleType, appoint, storeId, toAddUserName);
                    store.AddStaffMember(toAddUserName, role, appoint.UserName);
                    // store.SubscribeStaffMember(appoint, appointe);
                    transaction.Commit();
                }
                catch(Exception e) 
                {
                    transaction.Rollback();
                    throw new Exception(e.Message); 
                }
            }
            else
                throw new Exception("Store doesn't exist!");
        }

        public Store GetStore(int storeId){
            return _storeRepository.GetById(storeId);
        }

        public int GetMemberIDrByUserName(string userName)
        {
            return _clientManager.GetMemberIDrByUserName(userName); 
        }

        public Member GetMember(string userName)
        {
            return _clientManager.GetMember(userName); 
        }

        public void AddKeyWord(string keyWord, int storeId, int productId)
        {
            Store store = _storeRepository.GetById(storeId);
            if (store != null){
                store.AddKeyword(productId, keyWord);
            }
            else
                throw new Exception("Store doesn't exist!");
        }

        // policies ------------------------------------------------
        public void RemovePolicy(string identifier, int storeId, int policyID,string type)
        {
            _clientManager.CheckMemberIsLoggedIn(identifier);
            Store store = _storeRepository.GetById(storeId);
            if (store != null){
                Member activeMember = _clientManager.GetMemberByIdentifier(identifier);                
                store.RemovePolicy(activeMember.UserName, policyID, type);
            }
            else
                throw new Exception("Store doesn't exist!");
        }
        public int AddSimpleRule(string identifier, int storeId,string subject)
        {
            _clientManager.CheckMemberIsLoggedIn(identifier);
            Store store = _storeRepository.GetById(storeId);
            if (store != null){
                Member activeMember = _clientManager.GetMemberByIdentifier(identifier);                
                return store.AddSimpleRule(activeMember.UserName, subject);
            }
            else
                throw new Exception("Store doesn't exist!");
        }
        public int AddQuantityRule(string identifier, int storeId, string subject, int minQuantity, int maxQuantity)
        {
            _clientManager.CheckMemberIsLoggedIn(identifier);
            Store store = _storeRepository.GetById(storeId);
            if (store != null){
                Member activeMember = _clientManager.GetMemberByIdentifier(identifier);                
                return store.AddQuantityRule(activeMember.UserName, subject, minQuantity, maxQuantity);
            }
            else
                throw new Exception("Store doesn't exist!");
        }
        public int AddTotalPriceRule(string identifier, int storeId, string subject, int targetPrice)
        {
            _clientManager.CheckMemberIsLoggedIn(identifier);
            Store store = _storeRepository.GetById(storeId);
            if (store != null){
                Member activeMember = _clientManager.GetMemberByIdentifier(identifier);                
                return store.AddTotalPriceRule(activeMember.UserName, subject, targetPrice);
            }
            else
                throw new Exception("Store doesn't exist!");
        }
        public int AddCompositeRule(string identifier, int storeId, int Operator, List<int> rules)
        {
            _clientManager.CheckMemberIsLoggedIn(identifier);
            Store store = _storeRepository.GetById(storeId);
            if (store != null){
                Member activeMember = _clientManager.GetMemberByIdentifier(identifier);                
                LogicalOperator op = (LogicalOperator)Enum.ToObject(typeof(LogicalOperator), Operator);
                return store.AddCompositeRule(activeMember.UserName, op, rules);
            }
            else
                throw new Exception("Store doesn't exist!");
        }
        public void UpdateRuleSubject(string identifier, int storeId, int ruleId, string subject)
        {
            _clientManager.CheckMemberIsLoggedIn(identifier);
            Store store = _storeRepository.GetById(storeId);
            if (store != null){
                Member activeMember = _clientManager.GetMemberByIdentifier(identifier);                
                store.UpdateRuleSubject(activeMember.UserName, ruleId, subject);
            }
            else
                throw new Exception("Store doesn't exist!");
        }
        public void UpdateRuleQuantity(string identifier, int storeId, int ruleId, int minQuantity, int maxQuantity)
        {
            _clientManager.CheckMemberIsLoggedIn(identifier);
            Store store = _storeRepository.GetById(storeId);
            if (store != null){
                Member activeMember = _clientManager.GetMemberByIdentifier(identifier);                
                store.UpdateRuleQuantity(activeMember.UserName, ruleId, minQuantity, maxQuantity);
            }
            else
                throw new Exception("Store doesn't exist!");
        }
        public void UpdateRuleTargetPrice(string identifier, int storeId, int ruleId, int targetPrice)
        {
            _clientManager.CheckMemberIsLoggedIn(identifier);
            Store store = _storeRepository.GetById(storeId);
            if (store != null){
                Member activeMember = _clientManager.GetMemberByIdentifier(identifier);                
                store.UpdateRuleTargetPrice(activeMember.UserName, ruleId, targetPrice);
            }
            else
                throw new Exception("Store doesn't exist!");
        }
        public void UpdateCompositeOperator(string identifier, int storeId, int ruleId, int Operator)
        {
            _clientManager.CheckMemberIsLoggedIn(identifier);
            Store store = _storeRepository.GetById(storeId);
            if (store != null){
                Member activeMember = _clientManager.GetMemberByIdentifier(identifier);                
                LogicalOperator op = (LogicalOperator)Enum.ToObject(typeof(LogicalOperator), Operator);
                store.UpdateCompositeOperator(activeMember.UserName, ruleId, op);
            }
            else
                throw new Exception("Store doesn't exist!");
        }
        public void UpdateCompositeRules(string identifier, int storeId, int ruleId, List<int> rules)
        {
            _clientManager.CheckMemberIsLoggedIn(identifier);
            Store store = _storeRepository.GetById(storeId);
            if (store != null){
                Member activeMember = _clientManager.GetMemberByIdentifier(identifier);                
                store.UpdateCompositeRules(activeMember.UserName, ruleId, rules);
            }
            else
                throw new Exception("Store doesn't exist!");
        }

        public int AddPurchasePolicy(string identifier, int storeId, DateTime expirationDate, string subject, int ruleId)
        {
            _clientManager.CheckMemberIsLoggedIn(identifier);
            Store store = _storeRepository.GetById(storeId);
            if (store != null){
                Member activeMember = _clientManager.GetMemberByIdentifier(identifier);                
                return store.AddPurchasePolicy(activeMember.UserName, expirationDate, subject, ruleId);
            }
            else
                throw new Exception("Store doesn't exist!");
        }
        public int AddDiscountPolicy(string identifier, int storeId, DateTime expirationDate, string subject, int ruleId, double precentage)
        {
            _clientManager.CheckMemberIsLoggedIn(identifier);
            Store store = _storeRepository.GetById(storeId);
            if (store != null){
                Member activeMember = _clientManager.GetMemberByIdentifier(identifier);                
                return store.AddDiscountPolicy(activeMember.UserName, expirationDate, subject, ruleId, precentage);
            }
            else
                throw new Exception("Store doesn't exist!");
        }
        public int AddCompositePolicy(string identifier, int storeId, DateTime expirationDate, string subject, int Operator, List<int> policies)
        {
            _clientManager.CheckMemberIsLoggedIn(identifier);
            Store store = _storeRepository.GetById(storeId);
            if (store != null){
                Member activeMember = _clientManager.GetMemberByIdentifier(identifier);                
                NumericOperator op = (NumericOperator)Enum.ToObject(typeof(NumericOperator), Operator);
                return store.AddCompositePolicy(activeMember.UserName, expirationDate, subject, op, policies);
            }
            else
                throw new Exception("Store doesn't exist!");
        }

        public void NotificationOn(string identifier){
            _clientManager.CheckMemberIsLoggedIn(identifier);
            _clientManager.NotificationOn(identifier);
        }

        public void NotificationOff(string identifier){
            _clientManager.CheckMemberIsLoggedIn(identifier);
            _clientManager.NotificationOff(identifier);
        }

        public List<Store> GetMemberStores(string identifier)
        {
            var member = _clientManager.GetMemberByIdentifier(identifier);

            return _storeRepository.getAll()
                .Where(store => store.roles.Values.Any(role => role.userName == member.UserName))
                .ToList(); 
        }

        public Store GetMemberStore(string identifier, int storeId)
        {
            return GetMemberStores(identifier).Where(store => store.StoreId == storeId).FirstOrDefault();
        }

        public List<Store> GetStores()
        {
            return _storeRepository.getAll().ToList();
        }

        public List<Message> GetMemberNotifications(string identifier)
        {
            var member = _clientManager.GetMemberByIdentifier(identifier);
            return member.alerts.ToList();
        }

        public void SetMemberNotifications(string identifier, bool on)
        {
            _clientManager.SetMemberNotifications( identifier,  on);
            
        }

        public string GetTokenByUserName(string userName)
        {
            return _clientManager.GetTokenByUserName(userName);
        }


        private int UpdateStoreCounter()
        {
            List<Store> stores = StoreRepositoryRAM.GetInstance().getAll().ToList();
            if (stores.Count == 0)
                return 1;
            return stores.Max(store => store.StoreId) + 1;
        }
        // ---------------------------------------------------------

    }
}

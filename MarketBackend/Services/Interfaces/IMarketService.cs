using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketBackend.Domain.Market_Client;
using MarketBackend.Domain.Payment;
using MarketBackend.Domain.Shipping;
using MarketBackend.Services.Models;

namespace MarketBackend.Services.Interfaces
{
    public interface IMarketService
    {
        public Response<int> AddProduct(int storeId, string identifier, string name, string sellMethod, string description, double price, string category, int quantity, bool ageLimit);
        public Response RemoveProduct(int storeId,string identifier, int productId);
        public Response RemoveStaffMember(int storeId, string identifier, string toAddUserName);            
        public Response AddStaffMember(int storeId, string identifier, string roleName, string toAddUserName);   
        public Response AddManger(string identifier, int storeId, string toAddUserName);
        public Response RemoveManger(string identifier, int storeId, string toRemoveUserName);
        public Response AddOwner(string identifier, int storeId, string toAddUserName);
        public Response RegisterAsSystemAdmin(string username, string password, string email, int age);
        public Response RemoveOwner(string identifier, int storeId, string toRemoveUserName);
        public Response<List<Member>> GetOwners(int storeId);
        public Response<List<Member>> GetMangers(int storeId);
        public Response<Member> GetFounder(int storeId);
        public Response UpdateProductQuantity(int storeId, string identifier, int productId, int quantity); 
        public Response UpdateProductPrice(int storeId, string identifier,  int productId, double price);
        public Response CloseStore(string identifier, int storeId);
        public Response OpenStore(string identifier, int storeId);
        public Response<bool> IsAvailable(int productId);
        public Response RemovePermission(string identifier, int storeId, string toRemoveUserName, string permission);
        public Response AddPermission(string identifier, int storeId, string toAddUserName, string permission);
        public Response EditPurchasePolicy(int storeId);
        public Response<List<ProductResultDto>> SearchByKeywords(string keywords);
        public Response<List<ProductResultDto>> SearchByName(string name);
        public Response<List<ProductResultDto>> SearchByCategory(string category);
        public Response<string> GetInfo(int storeId);
        public Response<string> GetProductInfo(int storeId, int productId);
        public Response PurchaseCart(string identifier, PaymentDetails paymentDetails, ShippingDetails shippingDetails);
        public Response<List<PurchaseResultDto>> GetPurchaseHistoryByStore(int storeId, string identifier);
        public Response RemovePolicy(string identifier, int storeId, int policyID,string type);
        public Response<int> AddSimpleRule(string identifier, int storeId,string subject);
        public Response<int> AddQuantityRule(string identifier, int storeId, string subject, int minQuantity, int maxQuantity);
        public Response<int> AddTotalPriceRule(string identifier, int storeId, string subject, int targetPrice);
        public Response<int> AddCompositeRule(string identifier, int storeId, int Operator, List<int> rules);
        public Response UpdateRuleSubject(string identifier, int storeId, int ruleId, string subject);
        public Response UpdateRuleQuantity(string identifier, int storeId, int ruleId, int minQuantity, int maxQuantity);
        public Response UpdateRuleTargetPrice(string identifier, int storeId, int ruleId, int targetPrice);
        public Response UpdateCompositeOperator(string identifier, int storeId, int ruleId, int Operator);
        public Response UpdateCompositeRules(string identifier, int storeId, int ruleId, List<int> rules);
        public Response<int> AddPurchasePolicy(string identifier, int storeId, DateTime expirationDate, string subject, int ruleId);
        public Response<int> AddDiscountPolicy(string identifier, int storeId, DateTime expirationDate, string subject, int ruleId, double precentage);
        public Response<int> AddCompositePolicy(string identifier, int storeId, DateTime expirationDate, string subject, int Operator, List<int> policies);
        public Response<string> GetStoreById(int storeId);
        public Response<List<RuleResultDto>> GetStoreRules(int storeId, string identifier);
        public Response<List<DiscountPolicyResultDto>> GetStoreDiscountPolicies(int storeId, string identifier);
        public Response<List<PolicyResultDto>> GetStorePurchacePolicies(int storeId, string identifier);
        public Response AddKeyWord(string identifier, string keyWord, int storeId, int productId);
        public Response<List<StoreResultDto>> GetStores();
    }
}

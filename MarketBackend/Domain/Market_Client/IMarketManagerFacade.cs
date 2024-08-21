using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MarketBackend.Domain.Models;
using MarketBackend.Domain.Payment;
using MarketBackend.Domain.Shipping;

namespace MarketBackend.Domain.Market_Client
{
    public interface IMarketManagerFacade
    {
        void Register(string username, string password, string email, int age);
        void EnterAsGuest(string identifier);
        void PurchaseCart(string identifier, PaymentDetails paymentDetails, ShippingDetails shippingDetails);
        int CreateStore(string identifier, string storeName, string email, string phoneNum);
        bool ResToStoreManageReq(string identifier);
        bool ResToStoreOwnershipReq(string identifier); //respond to store ownership request
        void LogoutClient(string identifier);
        void RemoveFromCart(string identifier, int productId, int basketId, int quantity);
        ShoppingCart ViewCart(string identifier);
        void AddToCart(string identifier, int storeId, int productId, int quantity);
        string LoginClient(string username, string password);
        void ExitGuest(string identifier);
        // void UpdateProductDiscount(int productId, double discount);
        List<ShoppingCartHistory> GetPurchaseHistoryByClient(string userName);
        List<Purchase> GetPurchaseHistoryByStore(int storeId, string userName);
        Product AddProduct(int storeId, string identifier, string name, string sellMethod, string description, double price, string category, int quantity, bool ageLimit);
        void RemoveProduct(int storeId,string identifier, int productId);
        void RemoveStaffMember(int storeId, string identifier, string toRemoveUserName);
        void AddManger(string identifier, int storeId, string toAddUserName);
        void RemoveManger(string identifier, int storeId, string toRemoveUserName);
        void AddOwner(string identifier, int storeId, string toAddUserName);
        void RemoveOwner(string identifier, int storeId, string toRemoveUserName);
        List<Member> GetOwners(int storeId);
        List<Member> GetMangers(int storeId);
        Member GetFounder(int storeId);

        void UpdateProductQuantity(int storeId, string identifier, int productId, int quantity); 
        void UpdateProductPrice(int storeId, string identifier,  int productId, double price);
        void CloseStore(string identifier, int storeId);
        void OpenStore(string identifier, int storeId);
        bool IsAvailable(int storeId);
        void RemovePermission(string identifier, int storeId, string toRemoveUserName, Permission permission);
        void AddPermission(string identifier, int storeId, string toAddUserName, Permission permission);
        void EditPurchasePolicy(int storeId);
        HashSet<Product> SearchByKeyWords(string keywords);
        HashSet<Product> SearchByName(string name);
        HashSet<Product> SearchByCategory(string category);
        HashSet<Product> SearchByCategoryWithStore(int storeId, string category);
        HashSet<Product> SearchByKeyWordsWithStore(int storeId, string keywords);
        HashSet<Product> SearchByNameWithStore(int storeId, string name);
        void Filter (HashSet<Product> products, string category, double lowPrice, double highPrice, double lowProductRate, double highProductRate, double lowStoreRate, double highStoreRate);
        string GetProductInfo(int storId, int productId);
        public void AddStaffMember(int storeId, string identifier, string roleName, string toAddUserName);   
        public string GetInfo(int storeId);    
        public void RemovePolicy(string identifier, int storeId, int policyID,string type);
        public int AddSimpleRule(string identifier, int storeId,string subject);
        public int AddQuantityRule(string identifier, int storeId, string subject, int minQuantity, int maxQuantity);
        public int AddTotalPriceRule(string identifier, int storeId, string subject, int targetPrice);
        public int AddCompositeRule(string identifier, int storeId, int Operator, List<int> rules);
        public void UpdateRuleSubject(string identifier, int storeId, int ruleId, string subject);
        public void UpdateRuleQuantity(string identifier, int storeId, int ruleId, int minQuantity, int maxQuantity);
        public void UpdateRuleTargetPrice(string identifier, int storeId, int ruleId, int targetPrice);
        public void UpdateCompositeOperator(string identifier, int storeId, int ruleId, int Operator);
        public void UpdateCompositeRules(string identifier, int storeId, int ruleId, List<int> rules);
        public int AddPurchasePolicy(string identifier, int storeId, DateTime expirationDate, string subject, int ruleId);
        public int AddDiscountPolicy(string identifier, int storeId, DateTime expirationDate, string subject, int ruleId, double precentage);
        public int AddCompositePolicy(string identifier, int storeId, DateTime expirationDate, string subject, int Operator, List<int> policies);
        public void NotificationOn(string identifier);
        public void NotificationOff(string identifier);
    }
}

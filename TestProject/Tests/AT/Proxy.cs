using MarketBackend.Services;
using MarketBackend.Domain.Market_Client;
using MarketBackend.Domain.Payment;
using NLog;
using MarketBackend.Domain.Shipping;
using Moq;
using MarketBackend.Services.Models;

namespace MarketBackend.Tests.AT
{
    public class Proxy
    {
        MarketService marketService;
        ClientService clientService;

        int userId = 1;
        

        public Proxy(){
            var mockShippingSystem = new Mock<IShippingSystemFacade>();
            var mockPaymentSystem = new Mock<IPaymentSystemFacade>();
            mockPaymentSystem.Setup(pay =>pay.Connect()).Returns(true);
            mockShippingSystem.Setup(ship => ship.Connect()).Returns(true);
            mockPaymentSystem.Setup(pay =>pay.Pay(It.IsAny<PaymentDetails>(), It.IsAny<double>())).Returns(1);
            mockShippingSystem.Setup(ship =>ship.OrderShippment(It.IsAny<ShippingDetails>())).Returns(1);
            mockShippingSystem.SetReturnsDefault(true);
            mockPaymentSystem.SetReturnsDefault(true);            
            marketService = MarketService.GetInstance(mockShippingSystem.Object, mockPaymentSystem.Object);
            clientService = ClientService.GetInstance(mockShippingSystem.Object, mockPaymentSystem.Object);
            
        }

        public void Dispose(){
            marketService.Dispose();
            clientService.Dispose();
        }

        public int GetUserId(){
            string userIds = userId.ToString();
            userId = int.Parse(userIds) + 1;
            return userId;
        }

        public bool Login(string userName, string password){
            Response res = clientService.LoginClient(userName, password);
            return !res.ErrorOccured;
        }
        public string LoginWithToken(string userName, string password){
            Response<string> res = clientService.LoginClient(userName, password);
             if(res.ErrorOccured){
                throw new Exception(res.ErrorMessage);
             }
            return res.Value;
        }

        public bool EnterAsGuest(string identifier){
            Response res = clientService.EnterAsGuest(identifier);
            return !res.ErrorOccured;
        }

        public bool Register(string userName, string password, string email, int age){
            Response res = clientService.Register(userName, password, email, age);
            return !res.ErrorOccured;
        }

        public bool LogOut(string identifier){
            Response res = clientService.LogoutClient(identifier);
            return !res.ErrorOccured;
        }

        public bool SearchByKeywords(string keywords){
            Response res = marketService.SearchByKeywords(keywords);
            return !res.ErrorOccured;
        }

        public List<ProductResultDto> SearchByKey(string key){
            Response<List<ProductResultDto>> res = marketService.SearchByKeywords(key);
            return res.Value;
        }

        public bool SearchByName(string name){
            Response res = marketService.SearchByName(name);
            return !res.ErrorOccured;
        }
        public bool SearchByCategory(string category){
            Response res = marketService.SearchByCategory(category);
            return !res.ErrorOccured;
        }
        public bool OpenStore(string identifier, int storeId){
            Response res = marketService.OpenStore(identifier, storeId);
            return !res.ErrorOccured;
        }

        public bool CloseStore(string identifier, int storeId){
            Response res = marketService.CloseStore(identifier, storeId);
            return !res.ErrorOccured;
        }

        public bool GetInfo(int storeId){
            Response res = marketService.GetInfo(storeId);
            return !res.ErrorOccured;
        }

        public bool AddProduct(int storeId, string identifier, string name, string sellMethod, string description, double price, string category, int quantity, bool ageLimit)
        {
            Response res = marketService.AddProduct(storeId, identifier, name, sellMethod, description, price, category, quantity, ageLimit);
            return !res.ErrorOccured;
        }

        public bool RemoveProduct(int storeId,string identifier, int productId)
        {
            Response res = marketService.RemoveProduct(storeId, identifier, productId);
            return !res.ErrorOccured;
        }

        public bool AddToCart(string identifier, int storeId, int productId, int quantity){
            Response res = clientService.AddToCart(identifier, storeId, productId, quantity);
            return !res.ErrorOccured;
        }

        public bool RemoveFromCart(string identifier, int productId, int storeId, int quantity){
            Response res = clientService.RemoveFromCart(identifier, productId, storeId, quantity);
            return !res.ErrorOccured;
        }

        public bool PurchaseCart(string identifier, PaymentDetails paymentDetails, ShippingDetails shippingDetails){
            Response res = marketService.PurchaseCart(identifier, paymentDetails, shippingDetails);
            return !res.ErrorOccured;
        }

        // public bool UpdateProductDiscount(int productId, double discount) 
        // {
        //     Response res = marketService.UpdateProductDiscount(productId, discount);
        //     return !res.ErrorOccured;
        // }

        public bool UpdateProductPrice(int storeId, string identifier, int productId, double price)
        {
            Response res = marketService.UpdateProductPrice(storeId, identifier, productId, price);
            return !res.ErrorOccured;        
        }

        public bool UpdateProductQuantity(int storeId, string identifier, int productId, int quantity)
        {
            Response res = marketService.UpdateProductQuantity(storeId, identifier, productId, quantity);
            return !res.ErrorOccured;
        }

        public bool GetPurchaseHistory(int storeId, string identifier)
        {
            Response res = marketService.GetPurchaseHistoryByStore(storeId, identifier);
            return !res.ErrorOccured;
        }

        public bool RemoveStaffMember(int storeId, string identifier, string toRemoveUserName)
        {
            Response res = marketService.RemoveStaffMember(storeId, identifier, toRemoveUserName);
            return !res.ErrorOccured;
        }

        public bool AddStaffMember(int storeId, string identifier, Role role, string toAddUserName)
        {
            Response res = marketService.AddStaffMember(storeId, identifier, role.role.roleName.GetDescription(), toAddUserName);
            return !res.ErrorOccured;
        }

        public bool CreateStore(string identifier, string storeName, string email, string phoneNum){
            Response res = clientService.CreateStore(identifier, storeName, email, phoneNum);
            return !res.ErrorOccured;
        }
        
        public void InitiateSystemAdmin(){
            clientService.InitiateSystemAdmin();
        }

        public bool ExitGuest(string identifier){
            Response res = clientService.ExitGuest(identifier);
            return !res.ErrorOccured;
        }

        public bool AddOwner(string identifier, int storeId, string toAddUserName){
            Response res = marketService.AddOwner(identifier, storeId, toAddUserName);
            return !res.ErrorOccured;
        }

        public int GetMembeIDrByUserName(string userName){
            // Response res = 
            return clientService.GetMemberIDByUserName(userName);
            // return res.ErrorOccured ? -1 : int.Parse(res.ErrorMessage);
        }

        public bool AddKeyWord(string identifier, string keyWord, int storeId, int productId){
            Response res = marketService.AddKeyWord(identifier, keyWord, storeId, productId);
            return !res.ErrorOccured;
        }

        public bool AddSimpleRule(string identifier, int storeId,string subject){
            Response res = marketService.AddSimpleRule(identifier, storeId, subject);
            return !res.ErrorOccured;
        }

        public bool AddQuantityRule(string identifier, int storeId, string subject, int minQuantity, int maxQuantity){
            Response res = marketService.AddQuantityRule(identifier, storeId, subject, minQuantity, maxQuantity);
            return !res.ErrorOccured;
        }

        public bool AddTotalPriceRule(string identifier, int storeId, string subject, int targetPrice){
            Response res = marketService.AddTotalPriceRule(identifier, storeId, subject, targetPrice);
            return !res.ErrorOccured;
        }

        public bool GetPurchaseHistoryByClient(string userName){
            Response res = clientService.GetPurchaseHistoryByClient(userName);
            return !res.ErrorOccured;
        }

        public List<ShoppingCartResultDto> GetPurchaseHistory(string userName){
            Response<List<ShoppingCartResultDto>> res = clientService.GetPurchaseHistoryByClient(userName);
            return res.Value;
        }

        public bool GetProductInfo(int storeId, int productId){
            Response res = marketService.GetProductInfo(storeId, productId);
            return !res.ErrorOccured;
        }

        public Product GetProduct(int storeId, int productId){
            Response<Product> res = marketService.GetProduct(storeId, productId);
            return res.Value;
        }

        public List<Member> GetOwners(int storeId){
            Response<List<Member>> res = marketService.GetOwners(storeId);
            return res.Value;
        }

        public string GetStoreById(int storeId){
            Response<string> res = marketService.GetStoreById(storeId);
            return res.Value;
        }

        public Member GetMember(string userName){
            return clientService.GetMember(userName);
        }

        public List<RuleResultDto> GetStoreRules(int storeId, string identifier){
            Response<List<RuleResultDto>> res = marketService.GetStoreRules(storeId, identifier);
            return res.Value;
        }
    }
}
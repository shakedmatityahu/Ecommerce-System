using EcommerceAPI.initialize;
using MarketBackend.DAL;
using MarketBackend.DAL.DTO;
using MarketBackend.Domain.Market_Client;
using MarketBackend.Domain.Payment;
using MarketBackend.Domain.Shipping;
using MarketBackend.Services;
using Moq;

namespace UnitTests
{
    [TestClass]
    public class PolicyTest
    {
        MarketManagerFacade MMF;
        MarketService s;
        ClientService c;
        ClientManager CM;
        private Client _owner;
        private Store _Store;
        private Product _p1;
        private string token1;
        private string token2;
        string username1 = "nofar";
        string username2 = "noa";
        string username3 = "yonatan";

        [TestInitialize]
        public void Initialize(){
            MarketManagerFacade.Dispose();
            DBcontext.SetTestDB();
            DBcontext.GetInstance().Dispose();
            RuleRepositoryRAM.Dispose();
            DBcontext context = DBcontext.GetInstance();
            var mockShippingSystem = new Mock<IShippingSystemFacade>();
            var mockPaymentSystem = new Mock<IPaymentSystemFacade>();
            mockPaymentSystem.Setup(pay =>pay.Connect()).Returns(true);
            mockShippingSystem.Setup(ship => ship.Connect()).Returns(true);
            mockPaymentSystem.Setup(pay =>pay.Pay(It.IsAny<PaymentDetails>(), It.IsAny<double>())).Returns(1);
            mockShippingSystem.Setup(ship =>ship.OrderShippment(It.IsAny<ShippingDetails>())).Returns(1);
            mockShippingSystem.SetReturnsDefault(true);
            mockPaymentSystem.SetReturnsDefault(true);
            MarketService s = MarketService.GetInstance(mockShippingSystem.Object, mockPaymentSystem.Object);
            ClientService c = ClientService.GetInstance(mockShippingSystem.Object, mockPaymentSystem.Object);
            ClientManager CM = ClientManager.GetInstance();
            MarketManagerFacade MMF = MarketManagerFacade.GetInstance(mockShippingSystem.Object, mockPaymentSystem.Object);
            c.Register(username1, "12345", "nofar@gmail.com", 19);
            token1 = c.LoginClient(username1, "12345").Value;
            int storeId= MMF.CreateStore(token1, "shop1", "shop@gmail.com", "0502552798");
            _owner = CM.GetClientByIdentifier(token1);
            _Store = MMF.GetStore(storeId);
            _Store.AddProduct(username1, "Brush", "RegularSell" , "Brush", 4784, "hair", 21, false);
            _p1 = _Store.Products.ToList().Find((p) => p.ProductId == 11);
            c.Register( username2, "54321", "nofar@gmail.com", 18);
            token2 = c.LoginClient(username2, "54321").Value;
        }

        [TestCleanup]
        public void Cleanup()
        {
            DBcontext.GetInstance().Dispose();
            var mockShippingSystem = new Mock<IShippingSystemFacade>();
            var mockPaymentSystem = new Mock<IPaymentSystemFacade>();
            mockPaymentSystem.Setup(pay =>pay.Connect()).Returns(true);
            mockShippingSystem.Setup(ship => ship.Connect()).Returns(true);
            mockPaymentSystem.Setup(pay =>pay.Pay(It.IsAny<PaymentDetails>(), It.IsAny<double>())).Returns(1);
            mockShippingSystem.Setup(ship =>ship.OrderShippment(It.IsAny<ShippingDetails>())).Returns(1);
            mockShippingSystem.SetReturnsDefault(true);
            mockPaymentSystem.SetReturnsDefault(true);
            new Configurate(MarketService.GetInstance(mockShippingSystem.Object, mockPaymentSystem.Object), ClientService.GetInstance(mockShippingSystem.Object, mockPaymentSystem.Object)).Parse("initialize\\configTest.json");
            DBcontext.GetInstance().Dispose();
            MarketManagerFacade.Dispose();
            MarketService.GetInstance(mockShippingSystem.Object, mockPaymentSystem.Object).Dispose();
            ClientService.GetInstance(mockShippingSystem.Object, mockPaymentSystem.Object).Dispose();
            PolicyRepositoryRAM.Dispose();
            RuleRepositoryRAM.Dispose();
        }

        [TestMethod]
        public void AddPurchasePolicySuccess()
        {
            int minQuantity = 5;
            int maxQuantity = 20;
            _Store.AddQuantityRule(username1,"Brush", minQuantity, maxQuantity);
            QuantityRule r = (QuantityRule)RuleRepositoryRAM.GetInstance().GetById(11);
            Assert.IsTrue(r.MinQuantity == 5 && r.MaxQuantity == 20);
            Assert.IsTrue(r.Id == 11);
            _Store.AddPurchasePolicy(username1, new DateTime(2027, 1, 1), "Brush", 11);
            int count = _Store._purchasePolicyManager.Policies.Count;
            Assert.IsTrue(count == 1);
            Assert.IsTrue(_Store._purchasePolicyManager.Policies[11].Id == 11);
        }

        [TestMethod]
        public void AddPurchasePolicyRuleDoesntFound()
        {
            Assert.ThrowsException<Exception>(() => _Store.AddPurchasePolicy(username1, new DateTime(2027, 1, 1), "Brush", 11));
            int count = _Store._purchasePolicyManager.Policies.Count;
            Assert.IsTrue(count == 0);
        }

        [TestMethod]
        public void AddDiscountPolicySuccess()
        {
            int minQuantity = 5;
            int maxQuantity = 20;
            _Store.AddQuantityRule(username1,"Brush", minQuantity, maxQuantity);
            QuantityRule r = (QuantityRule)RuleRepositoryRAM.GetInstance().GetById(11);
            Assert.IsTrue(r.MinQuantity == 5 && r.MaxQuantity == 20);
            Assert.IsTrue(r.Id == 11);
            _Store.AddDiscountPolicy(username1, new DateTime(2027, 1, 1), "Brush", 11, 50);
            int count = _Store._discountPolicyManager.Policies.Count;
            Assert.IsTrue(count == 1);
            Assert.IsTrue(_Store._discountPolicyManager.Policies[11].Id == 11);
        }

        [TestMethod]
        public void AddDiscountPolicyRuleDoesntFound()
        {
            Assert.ThrowsException<Exception>(() => _Store.AddPurchasePolicy(username1, new DateTime(2027, 1, 1), "Brush", 11));
            int count = _Store._discountPolicyManager.Policies.Count;
            Assert.IsTrue(count == 0);
        }

        [TestMethod]
        public void AddDiscountCompositePolicySuccess()
        {
            int minQuantity = 5;
            int maxQuantity = 20;
            _Store.AddQuantityRule(username1,"Brush", minQuantity, maxQuantity);
            _Store.AddTotalPriceRule(username1, "Brush", 3000);
            _Store.AddDiscountPolicy(username1, new DateTime(2027, 1, 1), "Brush", 11, 50);
            _Store.AddDiscountPolicy(username1, new DateTime(2027, 1, 1), "Brush", 12, 20);
            NumericOperator n = NumericOperator.Add;
            _Store.AddCompositePolicy(username1, new DateTime(2027, 1, 1), "Brush", n, new List<int> { 11, 12 });
            int count = _Store._discountPolicyManager.Policies.Count;
            Assert.IsTrue(count == 1);
            Assert.IsTrue(_Store._discountPolicyManager.Policies[13].Id == 13);
        }


        [TestMethod]
        public void AddDiscountCompositePolicyRuleDoesntFound()
        {
            int minQuantity = 5;
            int maxQuantity = 20;
            _Store.AddQuantityRule(username1,"Brush", minQuantity, maxQuantity);
            _Store.AddTotalPriceRule(username1, "Brush", 3000);
            _Store.AddDiscountPolicy(username1, new DateTime(2027, 1, 1), "Brush", 11, 50);
            _Store.AddDiscountPolicy(username1, new DateTime(2027, 1, 1), "Brush", 12, 20);
            NumericOperator n = NumericOperator.Add;
            Assert.ThrowsException<Exception>(() => _Store.AddCompositePolicy(username1, new DateTime(2027, 1, 1), "Brush", n, new List<int> { 11, 13 }));
            int count = _Store._discountPolicyManager.Policies.Count;
            Assert.IsTrue(count == 2);
        }

        [TestMethod]
        public void RemovePurchasePolicySuccess()
        {
            int minQuantity = 5;
            int maxQuantity = 20;
            _Store.AddQuantityRule(username1,"Brush", minQuantity, maxQuantity);
            _Store.AddPurchasePolicy(username1, new DateTime(2027, 1, 1), "Brush", 11);
            int count = _Store._purchasePolicyManager.Policies.Count;
            Assert.IsTrue(count == 1);
            _Store.RemovePurchasePolicy(username1, 11);
            count = _Store._purchasePolicyManager.Policies.Count;
            Assert.IsTrue(count == 0);
        }

        [TestMethod]
        public void RemovePurchasePolicyRuleDoesntFound()
        {
            int minQuantity = 5;
            int maxQuantity = 20;
            _Store.AddQuantityRule(username1,"Brush", minQuantity, maxQuantity);
            _Store.AddPurchasePolicy(username1, new DateTime(2027, 1, 1), "Brush", 11);
            int count = _Store._purchasePolicyManager.Policies.Count;
            Assert.IsTrue(count == 1);
            Assert.ThrowsException<Exception>(() => _Store.RemovePurchasePolicy(username1, 12));
            Assert.IsTrue(count == 1);
        }
        [TestMethod]
        public void RemoveDiscountPolicySuccess()
        {
            int minQuantity = 5;
            int maxQuantity = 20;
            _Store.AddQuantityRule(username1,"Brush", minQuantity, maxQuantity);
            _Store.AddDiscountPolicy(username1, new DateTime(2027, 1, 1), "Brush", 11, 50);
            int count = _Store._discountPolicyManager.Policies.Count;
            Assert.IsTrue(count == 1);
            _Store.RemoveDiscountPolicy(username1, 11);
            count = _Store._discountPolicyManager.Policies.Count;
            Assert.IsTrue(count == 0);
        }

        [TestMethod]
        public void RemoveDiscountPolicyRuleDoesntFound()
        {
            int minQuantity = 5;
            int maxQuantity = 20;
            _Store.AddQuantityRule(username1,"Brush", minQuantity, maxQuantity);
            _Store.AddDiscountPolicy(username1, new DateTime(2027, 1, 1), "Brush", 11, 50);
            int count = _Store._discountPolicyManager.Policies.Count;
            Assert.IsTrue(count == 1);
            Assert.ThrowsException<Exception>(() => _Store.RemoveDiscountPolicy(username1, 12));
            Assert.IsTrue(count == 1);
        }

        [TestMethod]
        public void RemoveDiscountCompositePolicySuccess()
        {
            int minQuantity = 5;
            int maxQuantity = 20;
            _Store.AddQuantityRule(username1,"Brush", minQuantity, maxQuantity);
            _Store.AddTotalPriceRule(username1, "Brush", 3000);
            _Store.AddDiscountPolicy(username1, new DateTime(2027, 1, 1), "Brush", 11, 50);
            _Store.AddDiscountPolicy(username1, new DateTime(2027, 1, 1), "Brush", 12, 20);
            NumericOperator n = NumericOperator.Max;
            _Store.AddCompositePolicy(username1, new DateTime(2027, 1, 1), "Brush", n, new List<int> { 11, 12 });
            int count = _Store._discountPolicyManager.Policies.Count;
            Assert.IsTrue(count == 1);
            _Store.RemoveDiscountPolicy(username1, 13);
            count = _Store._discountPolicyManager.Policies.Count;
            Assert.IsTrue(count == 0);
        }

        [TestMethod]
        public void RemoveDiscountCompositePolicyRuleDoesntFound()
        {
            int minQuantity = 5;
            int maxQuantity = 20;
            _Store.AddQuantityRule(username1,"Brush", minQuantity, maxQuantity);
            _Store.AddTotalPriceRule(username1, "Brush", 3000);
            _Store.AddDiscountPolicy(username1, new DateTime(2027, 1, 1), "Brush", 11, 50);
            _Store.AddDiscountPolicy(username1, new DateTime(2027, 1, 1), "Brush", 12, 20);
            NumericOperator n = NumericOperator.Max;
            _Store.AddCompositePolicy(username1, new DateTime(2027, 1, 1), "Brush", n, new List<int> { 11, 12 });
            int count = _Store._discountPolicyManager.Policies.Count;
            Assert.IsTrue(count == 1);
            Assert.ThrowsException<Exception>(() => _Store.RemoveDiscountPolicy(username1, 12));
            Assert.IsTrue(count == 1);
        }
    }
}
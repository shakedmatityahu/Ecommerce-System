using System.IO.Compression;
using EcommerceAPI.initialize;
using MarketBackend.DAL.DTO;
using MarketBackend.Domain.Market_Client;
using MarketBackend.Domain.Models;
using MarketBackend.Domain.Payment;
using MarketBackend.Domain.Shipping;
using MarketBackend.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NLog;
using NuGet.Frameworks;

namespace MarketBackend.Tests.IT
{
    [TestClass()]
    public class PoliciesIT
    {
        string userName = "user1";
        string session1 = "1";
        string token1;
        string userName2 = "user2";
        string session2 = "2";
        string token2;
        string userPassword = "pass1";
        string pass2 = "pass2";
        string email1 = "printz@post.bgu.ac.il";
        string email2 = "hadaspr100@gmail.com";
        string wrongEmail = "@gmail.com";
        int userId;
        int productID1 = 11;
        string productName1 = "Banana";
        string category1 = "Fruit";
        string storeName = "Remi levi";
        string phoneNum = "0522768972";
        double price1 = 5.0;
        int quantity1 = 10;
        double discount1 = 0.5; 
        int userAge = 20;
        int userAge2 = 16;
        int basketId = 1;
        PaymentDetails paymentDetails = new PaymentDetails("ILS", "5326888878675678", "2027", "10", "101", "3190876789", "Hadas");
        ShippingDetails shippingDetails = new ShippingDetails("name",  "city",  "address",  "country",  "zipcode");
        private const int NumThreads = 10;
        private const int NumIterations = 100;
        string productname1 = "product1";
        private MarketManagerFacade marketManagerFacade;
        private ClientManager clientManager;
        string sellmethod = "RegularSell";
        string desc = "nice";
        int productCounter = 0;
        Mock<IShippingSystemFacade> mockShippingSystem;
        Mock<IPaymentSystemFacade> mockPaymentSystem;
        int or_operator = 0;
        int xor_operator = 1;
        int and_operator = 2;

        [TestInitialize]
        public void Setup()
        {
            MarketManagerFacade.Dispose();
            mockShippingSystem = new Mock<IShippingSystemFacade>();
            mockPaymentSystem = new Mock<IPaymentSystemFacade>();
            mockPaymentSystem.Setup(pay =>pay.Connect()).Returns(true);
            mockShippingSystem.Setup(ship => ship.Connect()).Returns(true);
            mockPaymentSystem.Setup(pay =>pay.Pay(It.IsAny<PaymentDetails>(), It.IsAny<double>())).Returns(1);
            mockShippingSystem.Setup(ship =>ship.OrderShippment(It.IsAny<ShippingDetails>())).Returns(1);
            mockShippingSystem.SetReturnsDefault(true);
            mockPaymentSystem.SetReturnsDefault(true);  
            new Configurate(MarketService.GetInstance(mockShippingSystem.Object, mockPaymentSystem.Object), ClientService.GetInstance(mockShippingSystem.Object, mockPaymentSystem.Object)).Parse("initialize\\configTest.json");
            DBcontext.GetInstance().Dispose();             
            marketManagerFacade = MarketManagerFacade.GetInstance(mockShippingSystem.Object, mockPaymentSystem.Object);
            clientManager = ClientManager.GetInstance();
            marketManagerFacade.InitiateSystemAdmin();
            marketManagerFacade.EnterAsGuest(session1);
            marketManagerFacade.Register(userName, userPassword, email1, userAge);
            token1 = marketManagerFacade.LoginClient(userName, userPassword);
            userId = marketManagerFacade.GetMemberIDrByUserName(userName);
            marketManagerFacade.CreateStore(token1, storeName, email1, phoneNum);
            marketManagerFacade.AddProduct(1, token1, productName1, sellmethod, desc, price1, category1, quantity1, false);
        }
        [TestCleanup]
        public void Cleanup()
        {
            DBcontext.GetInstance().Dispose();
            MarketManagerFacade.Dispose();
        }

        [TestMethod]
        public void AddCompositeRulePurchaseCart_success()
        {
            int rule1 = marketManagerFacade.AddQuantityRule(token1, 1, category1, 1, 10);
            int rule2 = marketManagerFacade.AddSimpleRule(token1, 1, storeName);
            List<int> rules = [rule1, rule2];
            marketManagerFacade.AddCompositeRule(token1, 1, or_operator, rules);
            Store store = marketManagerFacade.GetStore(1);
            Assert.IsTrue(store._rules.Count == 3);
            marketManagerFacade.AddToCart(token1, 1, productID1, 1);
            DateTime expirationDate = DateTime.Now.AddDays(2);
            marketManagerFacade.AddPurchasePolicy(token1, 1, expirationDate, storeName, rule1);
            marketManagerFacade.PurchaseCart(token1, paymentDetails, shippingDetails);
            Assert.AreEqual(1, store._history._purchases.Count,
            $"Expected puchase history count to be 1 but got {store._history._purchases.Count}");
        }

        [TestMethod]
        public void AddDiscountPurchaseCart_success()
        {
            int rule1 = marketManagerFacade.AddQuantityRule(token1, 1, category1, 1, 10);
            int rule2 = marketManagerFacade.AddSimpleRule(token1, 1, storeName);
            List<int> rules = [rule1, rule2];
            marketManagerFacade.AddCompositeRule(token1, 1, or_operator, rules);
            Store store = marketManagerFacade.GetStore(1);
            Assert.IsTrue(store._rules.Count == 3);
            marketManagerFacade.AddToCart(token1, 1, productID1, 1);
            DateTime expirationDate = DateTime.Now.AddDays(2);
            marketManagerFacade.AddDiscountPolicy(token1, 1, expirationDate, category1, rule1, 0.5);
            marketManagerFacade.PurchaseCart(token1, paymentDetails, shippingDetails);
            Assert.AreEqual(2.5, store._history._purchases[0].Price,
            $"Expected price to be 2.5 but got {store._history._purchases[0].Price}");
        }

        [TestMethod]
        public void PurchaseCart_Quantity_Role_Product_Fail()
        {
            marketManagerFacade.AddProduct(1, token1, "apple", "RegularSell", "nice", 5, category1, 200, false);
            int rule1 = marketManagerFacade.AddQuantityRule(token1, 1, "apple", 1, 5);
            DateTime expirationDate = DateTime.Now.AddDays(2);
            marketManagerFacade.AddPurchasePolicy(token1, 1, expirationDate, "apple", rule1);
            marketManagerFacade.AddToCart(token1, 1, 12, 10);
            Assert.ThrowsException<Exception>(() => marketManagerFacade.PurchaseCart(token1, paymentDetails, shippingDetails));
            Store store = marketManagerFacade.GetStore(1);
            Assert.AreEqual(0, store._history._purchases.Count,
            $"Expected puchase history count to be 0 but got {store._history._purchases.Count}");
        }

        [TestMethod]
        public void PurchaseCart_Quantity_Role__product_Success()
        {
            marketManagerFacade.AddProduct(1, token1, "apple", "RegularSell", "nice", 5, category1, 200, false);
            int rule1 = marketManagerFacade.AddQuantityRule(token1, 1, "apple", 1, 5);
            DateTime expirationDate = DateTime.Now.AddDays(2);
            marketManagerFacade.AddPurchasePolicy(token1, 1, expirationDate, "apple", rule1);
            marketManagerFacade.AddToCart(token1, 1, 12, 2);
            marketManagerFacade.PurchaseCart(token1, paymentDetails, shippingDetails);
            Store store = marketManagerFacade.GetStore(1);
            Assert.AreEqual(1, store._history._purchases.Count,
            $"Expected puchase history count to be 1 but got {store._history._purchases.Count}");
        }

        [TestMethod]
        public void PurchaseCart_Quantity_Role__simple_Fail()
        {
            marketManagerFacade.AddProduct(1, token1, "apple", "RegularSell", "nice", 5, category1, 200, false);
            int rule1 = marketManagerFacade.AddQuantityRule(token1, 1, storeName, 1, 5);
            DateTime expirationDate = DateTime.Now.AddDays(2);
            marketManagerFacade.AddPurchasePolicy(token1, 1, expirationDate, storeName, rule1);
            marketManagerFacade.AddToCart(token1, 1, 12, 2);
            marketManagerFacade.AddToCart(token1, 1, 11, 5);
            Assert.ThrowsException<Exception>(() => marketManagerFacade.PurchaseCart(token1, paymentDetails, shippingDetails));
            Store store = marketManagerFacade.GetStore(1);
            Assert.AreEqual(0, store._history._purchases.Count,
            $"Expected puchase history count to be 0 but got {store._history._purchases.Count}");
        }

        [TestMethod]
        public void PurchaseCart_Quantity_Role__category_Success()
        {
            marketManagerFacade.AddProduct(1, token1, "apple", "RegularSell", "nice", 5, category1, 200, false);
            int rule1 = marketManagerFacade.AddQuantityRule(token1, 1, category1, 1, 5);
            DateTime expirationDate = DateTime.Now.AddDays(2);
            marketManagerFacade.AddPurchasePolicy(token1, 1, expirationDate, category1, rule1);
            marketManagerFacade.AddToCart(token1, 1, 12, 2);
            marketManagerFacade.AddToCart(token1, 1, 11, 2);
            marketManagerFacade.PurchaseCart(token1, paymentDetails, shippingDetails);
            Store store = marketManagerFacade.GetStore(1);
            Assert.AreEqual(1, store._history._purchases.Count,
            $"Expected puchase history count to be 1 but got {store._history._purchases.Count}");
        }

        [TestMethod]
        public void PurchaseCart_Quantity_Role__category_Fail()
        {
            marketManagerFacade.AddProduct(1, token1, "apple", "RegularSell", "nice", 5, category1, 200, false);
            int rule1 = marketManagerFacade.AddQuantityRule(token1, 1, category1, 1, 5);
            DateTime expirationDate = DateTime.Now.AddDays(2);
            marketManagerFacade.AddPurchasePolicy(token1, 1, expirationDate, category1, rule1);
            marketManagerFacade.AddToCart(token1, 1, 12, 2);
            marketManagerFacade.AddToCart(token1, 1, 11, 5);
            Assert.ThrowsException<Exception>(() => marketManagerFacade.PurchaseCart(token1, paymentDetails, shippingDetails));
            Store store = marketManagerFacade.GetStore(1);
            Assert.AreEqual(0, store._history._purchases.Count,
            $"Expected puchase history count to be 0 but got {store._history._purchases.Count}");
        }

        [TestMethod]
        public void PurchaseCart_composite_rule__and_Success()
        {
            marketManagerFacade.AddProduct(1, token1, "apple", "RegularSell", "nice", 5, category1, 200, false);
            int rule1 = marketManagerFacade.AddQuantityRule(token1, 1, category1, 1, 5);
            int rule2 = marketManagerFacade.AddTotalPriceRule(token1, 1, category1, 5);
            List<int> rules = [rule1, rule2];
            int composite = marketManagerFacade.AddCompositeRule(token1, 1, and_operator, rules);
            DateTime expirationDate = DateTime.Now.AddDays(2);
            marketManagerFacade.AddPurchasePolicy(token1, 1, expirationDate, category1, composite);
            marketManagerFacade.AddToCart(token1, 1, 12, 2);
            marketManagerFacade.AddToCart(token1, 1, 11, 2);
            marketManagerFacade.PurchaseCart(token1, paymentDetails, shippingDetails);
            Store store = marketManagerFacade.GetStore(1);
            Assert.AreEqual(1, store._history._purchases.Count,
            $"Expected puchase history count to be 1 but got {store._history._purchases.Count}");
        }

        [TestMethod]
        public void PurchaseCart_composite_rule__and_Fail_OneNotTrue()
        {
            marketManagerFacade.AddProduct(1, token1, "apple", "RegularSell", "nice", 5, category1, 200, false);
            int rule1 = marketManagerFacade.AddQuantityRule(token1, 1, category1, 1, 5);
            int rule2 = marketManagerFacade.AddTotalPriceRule(token1, 1, category1, 5);
            List<int> rules = [rule1, rule2];
            int composite = marketManagerFacade.AddCompositeRule(token1, 1, and_operator, rules);
            DateTime expirationDate = DateTime.Now.AddDays(2);
            marketManagerFacade.AddPurchasePolicy(token1, 1, expirationDate, category1, composite);
            marketManagerFacade.AddToCart(token1, 1, 12, 2);
            marketManagerFacade.AddToCart(token1, 1, 11, 5);
            Assert.ThrowsException<Exception>(() => marketManagerFacade.PurchaseCart(token1, paymentDetails, shippingDetails));
            Store store = marketManagerFacade.GetStore(1);
            Assert.AreEqual(0, store._history._purchases.Count,
            $"Expected puchase history count to be 0 but got {store._history._purchases.Count}");
        }

        [TestMethod]
        public void PurchaseCart_composite_rule__or_Success()
        {
            marketManagerFacade.AddProduct(1, token1, "apple", "RegularSell", "nice", 5, category1, 200, false);
            int rule1 = marketManagerFacade.AddQuantityRule(token1, 1, category1, 1, 5);
            int rule2 = marketManagerFacade.AddTotalPriceRule(token1, 1, category1, 5);
            List<int> rules = [rule1, rule2];
            int composite = marketManagerFacade.AddCompositeRule(token1, 1, or_operator, rules);
            DateTime expirationDate = DateTime.Now.AddDays(2);
            marketManagerFacade.AddPurchasePolicy(token1, 1, expirationDate, category1, composite);
            marketManagerFacade.AddToCart(token1, 1, 12, 2);
            marketManagerFacade.AddToCart(token1, 1, 11, 2);
            marketManagerFacade.PurchaseCart(token1, paymentDetails, shippingDetails);
            Store store = marketManagerFacade.GetStore(1);
            Assert.AreEqual(1, store._history._purchases.Count,
            $"Expected puchase history count to be 1 but got {store._history._purchases.Count}");
        }

        [TestMethod]
        public void PurchaseCart_composite_rule__and_or_AllFalse()
        {
            marketManagerFacade.AddProduct(1, token1, "apple", "RegularSell", "nice", 5, category1, 200, false);
            int rule1 = marketManagerFacade.AddQuantityRule(token1, 1, category1, 1, 5);
            int rule2 = marketManagerFacade.AddTotalPriceRule(token1, 1, category1, 100);
            List<int> rules = [rule1, rule2];
            int composite = marketManagerFacade.AddCompositeRule(token1, 1, or_operator, rules);
            DateTime expirationDate = DateTime.Now.AddDays(2);
            marketManagerFacade.AddPurchasePolicy(token1, 1, expirationDate, category1, composite);
            marketManagerFacade.AddToCart(token1, 1, 12, 2);
            marketManagerFacade.AddToCart(token1, 1, 11, 5);
            Assert.ThrowsException<Exception>(() => marketManagerFacade.PurchaseCart(token1, paymentDetails, shippingDetails));
            Store store = marketManagerFacade.GetStore(1);
            Assert.AreEqual(0, store._history._purchases.Count,
            $"Expected puchase history count to be 0 but got {store._history._purchases.Count}");
        }

        [TestMethod]
        public void PurchaseCart_composite_rule__xor_Success1()
        {
            marketManagerFacade.AddProduct(1, token1, "apple", "RegularSell", "nice", 5, category1, 200, false);
            int rule1 = marketManagerFacade.AddQuantityRule(token1, 1, category1, 1, 5);
            int rule2 = marketManagerFacade.AddTotalPriceRule(token1, 1, category1, 5);
            List<int> rules = [rule1, rule2];
            int composite = marketManagerFacade.AddCompositeRule(token1, 1, xor_operator, rules);
            DateTime expirationDate = DateTime.Now.AddDays(2);
            marketManagerFacade.AddPurchasePolicy(token1, 1, expirationDate, category1, composite);
            marketManagerFacade.AddToCart(token1, 1, 12, 2);
            marketManagerFacade.AddToCart(token1, 1, 11, 5);
            marketManagerFacade.PurchaseCart(token1, paymentDetails, shippingDetails);
            Store store = marketManagerFacade.GetStore(1);
            Assert.AreEqual(1, store._history._purchases.Count,
            $"Expected puchase history count to be 1 but got {store._history._purchases.Count}");
        }

        [TestMethod]
        public void PurchaseCart_composite_rule__xor_Success2()
        {
            marketManagerFacade.AddProduct(1, token1, "apple", "RegularSell", "nice", 5, category1, 200, false);
            int rule1 = marketManagerFacade.AddQuantityRule(token1, 1, category1, 1, 5);
            int rule2 = marketManagerFacade.AddTotalPriceRule(token1, 1, category1, 100);
            List<int> rules = [rule1, rule2];
            int composite = marketManagerFacade.AddCompositeRule(token1, 1, xor_operator, rules);
            DateTime expirationDate = DateTime.Now.AddDays(2);
            marketManagerFacade.AddPurchasePolicy(token1, 1, expirationDate, category1, composite);
            marketManagerFacade.AddToCart(token1, 1, 12, 2);
            marketManagerFacade.AddToCart(token1, 1, 11, 2);
            marketManagerFacade.PurchaseCart(token1, paymentDetails, shippingDetails);
            Store store = marketManagerFacade.GetStore(1);
            Assert.AreEqual(1, store._history._purchases.Count,
            $"Expected puchase history count to be 1 but got {store._history._purchases.Count}");
        }

        [TestMethod]
        public void PurchaseCart_composite_rule__and_xor_AllFalse()
        {
            marketManagerFacade.AddProduct(1, token1, "apple", "RegularSell", "nice", 5, category1, 200, false);
            int rule1 = marketManagerFacade.AddQuantityRule(token1, 1, category1, 1, 5);
            int rule2 = marketManagerFacade.AddTotalPriceRule(token1, 1, category1, 5);
            List<int> rules = [rule1, rule2];
            int composite = marketManagerFacade.AddCompositeRule(token1, 1, xor_operator, rules);
            DateTime expirationDate = DateTime.Now.AddDays(2);
            marketManagerFacade.AddPurchasePolicy(token1, 1, expirationDate, category1, composite);
            marketManagerFacade.AddToCart(token1, 1, 12, 2);
            marketManagerFacade.AddToCart(token1, 1, 11, 2);
            Assert.ThrowsException<Exception>(() => marketManagerFacade.PurchaseCart(token1, paymentDetails, shippingDetails));
            Store store = marketManagerFacade.GetStore(1);
            Assert.AreEqual(0, store._history._purchases.Count,
            $"Expected puchase history count to be 0 but got {store._history._purchases.Count}");
        }

        [TestMethod]
        public void PurchaseCart_Discount_Role__category_Success()
        {
            marketManagerFacade.AddProduct(1, token1, "apple", "RegularSell", "nice", 5, category1, 200, false);
            int rule1 = marketManagerFacade.AddQuantityRule(token1, 1, category1, 1, 5);
            DateTime expirationDate = DateTime.Now.AddDays(2);
            marketManagerFacade.AddDiscountPolicy(token1, 1, expirationDate, category1, rule1, 0.5);
            marketManagerFacade.AddToCart(token1, 1, 12, 2);
            marketManagerFacade.AddToCart(token1, 1, 11, 2);
            marketManagerFacade.PurchaseCart(token1, paymentDetails, shippingDetails);
            Store store = marketManagerFacade.GetStore(1);
            Assert.AreEqual(1, store._history._purchases.Count,
            $"Expected puchase history count to be 1 but got {store._history._purchases.Count}");
            Assert.AreEqual(10, store._history._purchases[0].Price,
            $"Expected purchase price to be 10, but got {store._history._purchases[0].Price}");
        }

        [TestMethod]
        public void PurchaseCart_Discount_Role__category_Fail()
        {
            marketManagerFacade.AddProduct(1, token1, "apple", "RegularSell", "nice", 5, category1, 200, false);
            int rule1 = marketManagerFacade.AddQuantityRule(token1, 1, category1, 1, 5);
            DateTime expirationDate = DateTime.Now.AddDays(2);
            marketManagerFacade.AddDiscountPolicy(token1, 1, expirationDate, category1, rule1, 0.5);
            marketManagerFacade.AddToCart(token1, 1, 12, 2);
            marketManagerFacade.AddToCart(token1, 1, 11, 5);
            marketManagerFacade.PurchaseCart(token1, paymentDetails, shippingDetails);
            Store store = marketManagerFacade.GetStore(1);
            Assert.AreEqual(1, store._history._purchases.Count,
            $"Expected puchase history count to be 1 but got {store._history._purchases.Count}");
            Assert.AreEqual(35, store._history._purchases[0].Price,
            $"Expected purchase price to be 35, but got {store._history._purchases[0].Price}");
        }

        [TestMethod]
        public void PurchaseCart_Discount_rule__and_Success()
        {
            marketManagerFacade.AddProduct(1, token1, "apple", "RegularSell", "nice", 5, category1, 200, false);
            int rule1 = marketManagerFacade.AddQuantityRule(token1, 1, category1, 1, 5);
            int rule2 = marketManagerFacade.AddTotalPriceRule(token1, 1, category1, 5);
            List<int> rules = [rule1, rule2];
            int composite = marketManagerFacade.AddCompositeRule(token1, 1, and_operator, rules);
            DateTime expirationDate = DateTime.Now.AddDays(2);
            marketManagerFacade.AddPurchasePolicy(token1, 1, expirationDate, category1, composite);
            marketManagerFacade.AddToCart(token1, 1, 12, 2);
            marketManagerFacade.AddToCart(token1, 1, 11, 2);
            marketManagerFacade.PurchaseCart(token1, paymentDetails, shippingDetails);
            Store store = marketManagerFacade.GetStore(1);
            Assert.AreEqual(1, store._history._purchases.Count,
            $"Expected puchase history count to be 1 but got {store._history._purchases.Count}");
            Assert.AreEqual(20, store._history._purchases[0].Price,
            $"Expected purchase price to be 20, but got {store._history._purchases[0].Price}");
        }

        [TestMethod]
        public void PurchaseCart_Discount_rule__and_Fail_OneNotTrue()
        {
            marketManagerFacade.AddProduct(1, token1, "apple", "RegularSell", "nice", 5, category1, 200, false);
            int rule1 = marketManagerFacade.AddQuantityRule(token1, 1, category1, 1, 5);
            int rule2 = marketManagerFacade.AddTotalPriceRule(token1, 1, category1, 5);
            List<int> rules = [rule1, rule2];
            int composite = marketManagerFacade.AddCompositeRule(token1, 1, and_operator, rules);
            DateTime expirationDate = DateTime.Now.AddDays(2);
            marketManagerFacade.AddDiscountPolicy(token1, 1, expirationDate, category1, composite, 0.5);
            marketManagerFacade.AddToCart(token1, 1, 12, 2);
            marketManagerFacade.AddToCart(token1, 1, 11, 5);
            marketManagerFacade.PurchaseCart(token1, paymentDetails, shippingDetails);
            Store store = marketManagerFacade.GetStore(1);
            Assert.AreEqual(1, store._history._purchases.Count,
            $"Expected puchase history count to be 1 but got {store._history._purchases.Count}");
            Assert.AreEqual(35, store._history._purchases[0].Price,
            $"Expected purchase price to be 35, but got {store._history._purchases[0].Price}");
        }

        [TestMethod]
        public void PurchaseCart_Discount_rule__or_Success()
        {
            marketManagerFacade.AddProduct(1, token1, "apple", "RegularSell", "nice", 5, category1, 200, false);
            int rule1 = marketManagerFacade.AddQuantityRule(token1, 1, category1, 1, 5);
            int rule2 = marketManagerFacade.AddTotalPriceRule(token1, 1, category1, 5);
            List<int> rules = [rule1, rule2];
            int composite = marketManagerFacade.AddCompositeRule(token1, 1, or_operator, rules);
            DateTime expirationDate = DateTime.Now.AddDays(2);
            marketManagerFacade.AddDiscountPolicy(token1, 1, expirationDate, category1, composite, 0.5);
            marketManagerFacade.AddToCart(token1, 1, 12, 2);
            marketManagerFacade.AddToCart(token1, 1, 11, 2);
            marketManagerFacade.PurchaseCart(token1, paymentDetails, shippingDetails);
            Store store = marketManagerFacade.GetStore(1);
            Assert.AreEqual(1, store._history._purchases.Count,
            $"Expected puchase history count to be 1 but got {store._history._purchases.Count}");
            Assert.AreEqual(10, store._history._purchases[0].Price,
            $"Expected purchase price to be 10, but got {store._history._purchases[0].Price}");
        }

        [TestMethod]
        public void PurchaseCart_Discount_rule__and_or_AllFalse()
        {
            marketManagerFacade.AddProduct(1, token1, "apple", "RegularSell", "nice", 5, category1, 200, false);
            int rule1 = marketManagerFacade.AddQuantityRule(token1, 1, category1, 1, 5);
            int rule2 = marketManagerFacade.AddTotalPriceRule(token1, 1, category1, 100);
            List<int> rules = [rule1, rule2];
            int composite = marketManagerFacade.AddCompositeRule(token1, 1, or_operator, rules);
            DateTime expirationDate = DateTime.Now.AddDays(2);
            marketManagerFacade.AddDiscountPolicy(token1, 1, expirationDate, category1, composite, 0.5);
            marketManagerFacade.AddToCart(token1, 1, 12, 2);
            marketManagerFacade.AddToCart(token1, 1, 11, 5);
            marketManagerFacade.PurchaseCart(token1, paymentDetails, shippingDetails);
            Store store = marketManagerFacade.GetStore(1);
            Assert.AreEqual(1, store._history._purchases.Count,
            $"Expected puchase history count to be 1 but got {store._history._purchases.Count}");
            Assert.AreEqual(35, store._history._purchases[0].Price,
            $"Expected purchase price to be 35, but got {store._history._purchases[0].Price}");
        }

        [TestMethod]
        public void PurchaseCart_Discount_rule__xor_Success1()
        {
            marketManagerFacade.AddProduct(1, token1, "apple", "RegularSell", "nice", 5, category1, 200, false);
            int rule1 = marketManagerFacade.AddQuantityRule(token1, 1, category1, 1, 5);
            int rule2 = marketManagerFacade.AddTotalPriceRule(token1, 1, category1, 5);
            List<int> rules = [rule1, rule2];
            int composite = marketManagerFacade.AddCompositeRule(token1, 1, xor_operator, rules);
            DateTime expirationDate = DateTime.Now.AddDays(2);
            marketManagerFacade.AddDiscountPolicy(token1, 1, expirationDate, category1, composite, 0.5);
            marketManagerFacade.AddToCart(token1, 1, 12, 2);
            marketManagerFacade.AddToCart(token1, 1, 11, 5);
            marketManagerFacade.PurchaseCart(token1, paymentDetails, shippingDetails);
            Store store = marketManagerFacade.GetStore(1);
            Assert.AreEqual(1, store._history._purchases.Count,
            $"Expected puchase history count to be 1 but got {store._history._purchases.Count}");
            Assert.AreEqual(17.5, store._history._purchases[0].Price,
            $"Expected purchase price to be 17.5, but got {store._history._purchases[0].Price}");
        }

        [TestMethod]
        public void PurchaseCart_Discount_rule__xor_Success2()
        {
            marketManagerFacade.AddProduct(1, token1, "apple", "RegularSell", "nice", 5, category1, 200, false);
            int rule1 = marketManagerFacade.AddQuantityRule(token1, 1, category1, 1, 5);
            int rule2 = marketManagerFacade.AddTotalPriceRule(token1, 1, category1, 100);
            List<int> rules = [rule1, rule2];
            int composite = marketManagerFacade.AddCompositeRule(token1, 1, xor_operator, rules);
            DateTime expirationDate = DateTime.Now.AddDays(2);
            marketManagerFacade.AddDiscountPolicy(token1, 1, expirationDate, category1, composite, 0.5);
            marketManagerFacade.AddToCart(token1, 1, 12, 2);
            marketManagerFacade.AddToCart(token1, 1, 11, 2);
            marketManagerFacade.PurchaseCart(token1, paymentDetails, shippingDetails);
            Store store = marketManagerFacade.GetStore(1);
            Assert.AreEqual(1, store._history._purchases.Count,
            $"Expected puchase history count to be 1 but got {store._history._purchases.Count}");
            Assert.AreEqual(10, store._history._purchases[0].Price,
            $"Expected purchase price to be 10, but got {store._history._purchases[0].Price}");
        }

        [TestMethod]
        public void PurchaseCart_Discount_rule__and_xor_AllFalse()
        {
            marketManagerFacade.AddProduct(1, token1, "apple", "RegularSell", "nice", 5, category1, 200, false);
            int rule1 = marketManagerFacade.AddQuantityRule(token1, 1, category1, 1, 5);
            int rule2 = marketManagerFacade.AddTotalPriceRule(token1, 1, category1, 5);
            List<int> rules = [rule1, rule2];
            int composite = marketManagerFacade.AddCompositeRule(token1, 1, xor_operator, rules);
            DateTime expirationDate = DateTime.Now.AddDays(2);
            marketManagerFacade.AddDiscountPolicy(token1, 1, expirationDate, category1, composite, 0.5);
            marketManagerFacade.AddToCart(token1, 1, 12, 2);
            marketManagerFacade.AddToCart(token1, 1, 11, 2);
            marketManagerFacade.PurchaseCart(token1, paymentDetails, shippingDetails);
            Store store = marketManagerFacade.GetStore(1);
            Assert.AreEqual(1, store._history._purchases.Count,
            $"Expected puchase history count to be 1 but got {store._history._purchases.Count}");
            Assert.AreEqual(20, store._history._purchases[0].Price,
            $"Expected purchase price to be 20, but got {store._history._purchases[0].Price}");
        }

        [TestMethod]
        public void PurchaseCart_Discount_Success_Purchase_fail()
        {
            marketManagerFacade.AddProduct(1, token1, "apple", "RegularSell", "nice", 5, category1, 200, false);
            int rule1 = marketManagerFacade.AddQuantityRule(token1, 1, category1, 1, 5);
            int rule2 = marketManagerFacade.AddTotalPriceRule(token1, 1, category1, 5);
            DateTime expirationDate = DateTime.Now.AddDays(2);
            marketManagerFacade.AddPurchasePolicy(token1, 1, expirationDate, category1, rule1);
            marketManagerFacade.AddDiscountPolicy(token1, 1, expirationDate, category1, rule2, 0.5);
            marketManagerFacade.AddToCart(token1, 1, 12, 2);
            marketManagerFacade.AddToCart(token1, 1, 11, 5);
            Assert.ThrowsException<Exception>(() => marketManagerFacade.PurchaseCart(token1, paymentDetails, shippingDetails));
            Store store = marketManagerFacade.GetStore(1);
            Assert.AreEqual(0, store._history._purchases.Count,
            $"Expected puchase history count to be 0 but got {store._history._purchases.Count}");
        }

        [TestMethod]
        public void PurchaseCart_Discount_Fail_Purchase_Success()
        {
            marketManagerFacade.AddProduct(1, token1, "apple", "RegularSell", "nice", 5, category1, 200, false);
            int rule1 = marketManagerFacade.AddQuantityRule(token1, 1, category1, 1, 5);
            int rule2 = marketManagerFacade.AddTotalPriceRule(token1, 1, category1, 5);
            DateTime expirationDate = DateTime.Now.AddDays(2);
            marketManagerFacade.AddPurchasePolicy(token1, 1, expirationDate, category1, rule2);
            marketManagerFacade.AddDiscountPolicy(token1, 1, expirationDate, category1, rule1, 0.5);
            marketManagerFacade.AddToCart(token1, 1, 12, 2);
            marketManagerFacade.AddToCart(token1, 1, 11, 5);
            marketManagerFacade.PurchaseCart(token1, paymentDetails, shippingDetails);
            Store store = marketManagerFacade.GetStore(1);
            Assert.AreEqual(1, store._history._purchases.Count,
            $"Expected puchase history count to be 1 but got {store._history._purchases.Count}");
            Assert.AreEqual(35, store._history._purchases[0].Price,
            $"Expected purchase price to be 35, but got {store._history._purchases[0].Price}");
        }

        [TestMethod]
        public void PurchaseCart_Discount_Success_Purchase_Success()
        {
            marketManagerFacade.AddProduct(1, token1, "apple", "RegularSell", "nice", 5, category1, 200, false);
            int rule2 = marketManagerFacade.AddTotalPriceRule(token1, 1, category1, 5);
            DateTime expirationDate = DateTime.Now.AddDays(2);
            marketManagerFacade.AddPurchasePolicy(token1, 1, expirationDate, category1, rule2);
            marketManagerFacade.AddDiscountPolicy(token1, 1, expirationDate, category1, rule2, 0.5);
            marketManagerFacade.AddToCart(token1, 1, 12, 2);
            marketManagerFacade.AddToCart(token1, 1, 11, 5);
            marketManagerFacade.PurchaseCart(token1, paymentDetails, shippingDetails);
            Store store = marketManagerFacade.GetStore(1);
            Assert.AreEqual(1, store._history._purchases.Count,
            $"Expected puchase history count to be 1 but got {store._history._purchases.Count}");
            Assert.AreEqual(17.5, store._history._purchases[0].Price,
            $"Expected purchase price to be 17.5, but got {store._history._purchases[0].Price}");
        }

        [TestMethod]
        public void PurchaseCart_Composite_Policies_add()
        {
            marketManagerFacade.AddProduct(1, token1, "apple", "RegularSell", "nice", 5, category1, 200, false);
            int rule1 = marketManagerFacade.AddTotalPriceRule(token1, 1, category1, 10);
            int rule2 = marketManagerFacade.AddQuantityRule(token1, 1, category1, 1, 5);
            DateTime expirationDate = DateTime.Now.AddDays(2);
            int policy1 = marketManagerFacade.AddDiscountPolicy(token1, 1, expirationDate, category1, rule2, 0.5);
            int policy2 = marketManagerFacade.AddDiscountPolicy(token1, 1, expirationDate, category1, rule1, 0.1);
            List<int> policies = [policy1, policy2];
            marketManagerFacade.AddCompositePolicy(token1, 1, expirationDate, category1, 0, policies);
            marketManagerFacade.AddToCart(token1, 1, 12, 2);
            marketManagerFacade.AddToCart(token1, 1, 11, 2);
            marketManagerFacade.PurchaseCart(token1, paymentDetails, shippingDetails);
            Store store = marketManagerFacade.GetStore(1);
            Assert.AreEqual(1, store._history._purchases.Count,
            $"Expected puchase history count to be 1 but got {store._history._purchases.Count}");
            Assert.AreEqual(8, store._history._purchases[0].Price,
            $"Expected purchase price to be 8, but got {store._history._purchases[0].Price}");
        }

        [TestMethod]
        public void PurchaseCart_Composite_Policies_max()
        {
            marketManagerFacade.AddProduct(1, token1, "apple", "RegularSell", "nice", 5, category1, 200, false);
            int rule1 = marketManagerFacade.AddTotalPriceRule(token1, 1, category1, 10);
            int rule2 = marketManagerFacade.AddQuantityRule(token1, 1, category1, 1, 5);
            DateTime expirationDate = DateTime.Now.AddDays(2);
            int policy1 = marketManagerFacade.AddDiscountPolicy(token1, 1, expirationDate, category1, rule2, 0.5);
            int policy2 = marketManagerFacade.AddDiscountPolicy(token1, 1, expirationDate, category1, rule1, 0.1);
            List<int> policies = [policy1, policy2];
            marketManagerFacade.AddCompositePolicy(token1, 1, expirationDate, category1, 1, policies);
            marketManagerFacade.AddToCart(token1, 1, 12, 2);
            marketManagerFacade.AddToCart(token1, 1, 11, 2);
            marketManagerFacade.PurchaseCart(token1, paymentDetails, shippingDetails);
            Store store = marketManagerFacade.GetStore(1);
            Assert.AreEqual(1, store._history._purchases.Count,
            $"Expected puchase history count to be 1 but got {store._history._purchases.Count}");
            Assert.AreEqual(10, store._history._purchases[0].Price,
            $"Expected purchase price to be 10, but got {store._history._purchases[0].Price}");
        }

        // [TestMethod]
        // public void RunMultyTimes()
        // {
        //     for (int i=0; i<5; i++){
        //         AddCompositeRulePurchaseCart_success();
        //         Cleanup();
        //         Setup();
        //         AddDiscountPurchaseCart_success();
        //         Cleanup();
        //         Setup();
        //         PurchaseCart_Quantity_Role_Product_Fail();
        //         Cleanup();
        //         Setup();
        //         PurchaseCart_Quantity_Role__product_Success();
        //         Cleanup();
        //         Setup();
        //         PurchaseCart_Quantity_Role__simple_Fail();
        //         Cleanup();
        //         Setup();
        //         PurchaseCart_Quantity_Role__category_Success();
        //         Cleanup();
        //         Setup();
        //         PurchaseCart_Quantity_Role__category_Fail();
        //         Cleanup();
        //         Setup();
        //         PurchaseCart_composite_rule__and_Success();
        //         Cleanup();
        //         Setup();
        //         PurchaseCart_composite_rule__and_Fail_OneNotTrue();
        //         Cleanup();
        //         Setup();
        //         PurchaseCart_composite_rule__or_Success();
        //         Cleanup();
        //         Setup();
        //         PurchaseCart_composite_rule__and_or_AllFalse();
        //         Cleanup();
        //         Setup();
        //         PurchaseCart_composite_rule__xor_Success1();
        //         Cleanup();
        //         Setup();
        //         PurchaseCart_composite_rule__xor_Success2();
        //         Cleanup();
        //         Setup();
        //         PurchaseCart_composite_rule__and_xor_AllFalse();
        //         Cleanup();
        //         Setup();
        //         PurchaseCart_Discount_Role__category_Success();
        //         Cleanup();
        //         Setup();
        //         PurchaseCart_Discount_Role__category_Fail();
        //         Cleanup();
        //         Setup();
        //         PurchaseCart_Discount_rule__and_Success();
        //         Cleanup();
        //         Setup();
        //         PurchaseCart_Discount_rule__and_Fail_OneNotTrue();
        //         Cleanup();
        //         Setup();
        //         PurchaseCart_Discount_rule__or_Success();
        //         Cleanup();
        //         Setup();
        //         PurchaseCart_Discount_rule__and_or_AllFalse();
        //         Cleanup();
        //         Setup();
        //         PurchaseCart_Discount_rule__xor_Success1();
        //         Cleanup();
        //         Setup();
        //         PurchaseCart_Discount_rule__xor_Success2();
        //         Cleanup();
        //         Setup();
        //         PurchaseCart_Discount_rule__and_xor_AllFalse();
        //         Cleanup();
        //         Setup();
        //         PurchaseCart_Discount_Success_Purchase_fail();
        //         Cleanup();
        //         Setup();
        //         PurchaseCart_Discount_Fail_Purchase_Success();
        //         Cleanup();
        //         Setup();
        //         PurchaseCart_Discount_Success_Purchase_Success();
        //         Cleanup();
        //         Setup();
        //         PurchaseCart_Composite_Policies_add();
        //         Cleanup();
        //         Setup();
        //         PurchaseCart_Composite_Policies_max();
        //         Cleanup();
        //         Setup();
        //     }
        // }
    }
}
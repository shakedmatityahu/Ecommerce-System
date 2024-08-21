using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Mail;
using MarketBackend.Domain.Market_Client;
using MarketBackend.DAL;
using MarketBackend.DAL.DTO;
using MarketBackend.Services;
using Moq;
using MarketBackend.Domain.Shipping;
using MarketBackend.Domain.Payment;
using EcommerceAPI.initialize;

namespace UnitTests
{
    [TestClass]
    public class ClientTest
    {
        private MarketManagerFacade marketManagerFacade;
        string userName = "user1";
        string session1 = "1";
        string token1;
        string userName2 = "user2";
        string session2 = "2";
        string token2;

        string userName3 = "user3";
        string session3 = "3";
        string token3;

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
        private ClientManager clientManager;
        string sellmethod = "RegularSell";
        string desc = "nice";
        int productCounter = 0;
        int storeId = 1;
        int userId2;

        [TestInitialize]
        public void SetUp()
        {
            MarketManagerFacade.Dispose();
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
            marketManagerFacade = MarketManagerFacade.GetInstance(mockShippingSystem.Object, mockPaymentSystem.Object);
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
        public void TestAddToCart()
        {
            var client = new Guest(1); // Testing with Guest as an example
            client.AddToCart(1, 11, 10); // basket, productId, quantity
            var productsInBasket = BasketRepositoryRAM.GetInstance().getBasketsByCartId(client.Cart._shoppingCartId).Where(basket => basket._storeId == 1).FirstOrDefault()?.products;
            Assert.AreEqual(10, productsInBasket?[11],
            $"Expected products in basket to be 10, but got {productsInBasket?[11]}");
        }

        [TestMethod]
        public void TestRemoveFromCart()
        {
            var client = new Guest(1);
            client.AddToCart(1, 11, 10);
            var basket = BasketRepositoryRAM.GetInstance().getBasketsByCartId(client.Cart._shoppingCartId).Where(basket => basket._storeId == 1).FirstOrDefault();
            client.RemoveFromCart(1, 11, 10);
            var productsInBasket = basket.products;
            Assert.AreEqual(0, productsInBasket.Count,
            $"Expected count of products to be 0 but got {productsInBasket.Count}");
        }
    }

    [TestClass]
    public class MemberTest
    {
        private MarketManagerFacade marketManagerFacade;
        string userName = "user1";
        string session1 = "1";
        string token1;
        string userName2 = "user2";
        string session2 = "2";
        string token2;

        string userName3 = "user3";
        string session3 = "3";
        string token3;

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
        private ClientManager clientManager;
        string sellmethod = "RegularSell";
        string desc = "nice";
        int productCounter = 0;
        int storeId = 1;
        int userId2;
        private Member member;

        [TestInitialize]
        public void SetUp()
        {
            DBcontext.SetTestDB();
            DBcontext.GetInstance().Dispose();
            MarketManagerFacade.Dispose();
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
            marketManagerFacade = MarketManagerFacade.GetInstance(mockShippingSystem.Object, mockPaymentSystem.Object);
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
        }

        [TestMethod]
        public void MemberLoginTest()
        {
            Member member = ClientManager.GetInstance().GetMemberByIdentifier(token1);
            member.IsLoggedIn = true;
            Assert.IsTrue(member.IsLoggedIn);
        }

    }

    [TestClass]
    public class GuestTest
    {
        private MarketManagerFacade marketManagerFacade;
        string userName = "user1";
        string session1 = "1";
        string token1;
        string userName2 = "user2";
        string session2 = "2";
        string token2;

        string userName3 = "user3";
        string session3 = "3";
        string token3;

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
        private ClientManager clientManager;
        string sellmethod = "RegularSell";
        string desc = "nice";
        int productCounter = 0;
        int storeId = 1;
        int userId2;

        [TestInitialize]
        public void SetUp()
        {                
            MarketManagerFacade.Dispose();
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
            marketManagerFacade = MarketManagerFacade.GetInstance(mockShippingSystem.Object, mockPaymentSystem.Object);
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
        }
        
        [TestMethod]
        public void GuestCreationTest()
        {
            var guest = new Guest(1);
            Assert.AreEqual(1, guest.Id,
            $"Expected guest id to be 1 but got {guest.Id}");
        }

        [TestMethod]
        public void AddToCart()
        {
            var guest = new Guest(1);
            guest.AddToCart(1, 11, 1);
            Assert.AreEqual(1, guest.Cart.GetBaskets().Count);
        }

        [TestMethod]
        public void PurchaseBasket()
        {
            var guest = new Guest(1);
            guest.AddToCart(1, 11, 1);
            Basket basket = guest.Cart.GetBaskets()[1];
            guest.PurchaseBasket(basket);
            Assert.AreEqual(0, guest.Cart.GetBaskets().Count);
        }
    }
}

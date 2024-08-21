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

namespace MarketBackend.Tests.IT
{
    [TestClass()]
    public class MarketIT
    {
        // Define test data
        string userName = "user1";
        string session1 = "1";
        string token1;
        string userName2 = "user2";
        string session2 = "2";
        string token2;
        string userPassword = "pass1";
        string email1 = "printz@post.bgu.ac.il";
        string email2 = "hadaspr100@gmail.com";
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
        PaymentDetails paymentDetails = new PaymentDetails("ILS", "5326888878675678", "2027", "10", "101", "3190876789", "Hadas");
        ShippingDetails shippingDetails = new ShippingDetails("name", "city", "address", "country", "zipcode");
        private MarketManagerFacade marketManagerFacade;
        private ClientManager clientManager;
        string sellmethod = "RegularSell";
        string desc = "nice";
        Mock<IShippingSystemFacade> mockShippingSystem;
        Mock<IPaymentSystemFacade> mockPaymentSystem;

        [TestInitialize]
        public void Setup()
        {            
            MarketManagerFacade.Dispose();
            mockShippingSystem = new Mock<IShippingSystemFacade>();
            mockPaymentSystem = new Mock<IPaymentSystemFacade>();
            mockPaymentSystem.Setup(pay => pay.Connect()).Returns(true);
            mockShippingSystem.Setup(ship => ship.Connect()).Returns(true);
            mockPaymentSystem.Setup(pay => pay.Pay(It.IsAny<PaymentDetails>(), It.IsAny<double>())).Returns(1);
            mockShippingSystem.Setup(ship => ship.OrderShippment(It.IsAny<ShippingDetails>())).Returns(1);
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
        public void PurchaseCartGuest()
        {
            marketManagerFacade.EnterAsGuest(session2);
            Client guest = clientManager.GetClientByIdentifier(session2);
            marketManagerFacade.AddToCart(session2, 1, 11, 1);
            marketManagerFacade.PurchaseCart(session2, paymentDetails, shippingDetails);
        }

        [TestMethod]
        public void AddProductToShop()
        {
            Store store = marketManagerFacade.GetStore(1);
            Assert.IsTrue(store.Products.Count() > 0, "Expected the store to have products after adding one.");
        }

        [TestMethod]
        public void RemoveProductFromShop()
        {
            Store store = marketManagerFacade.GetStore(1);
            Assert.IsTrue(store.Products.Count() > 0, "Expected the store to have products before removal.");
            int prodId = 11;
            marketManagerFacade.RemoveProduct(1, token1, prodId);
            store = marketManagerFacade.GetStore(1);
            Assert.IsTrue(store.Products.Count == 0, "Expected the store to have no products after removal.");
        }

        [TestMethod]
        public void AddProductToBasket()
        {
            marketManagerFacade.AddToCart(token1, 1, productID1, 1);
            Client client = clientManager.GetClientByIdentifier(token1);
            Dictionary<int, Basket> baskets = client.Cart.GetBaskets();
            Basket relevantBasket = baskets[1];
            Assert.IsTrue(relevantBasket.products[productID1] == 1, "Expected the product to be added to the basket.");
        }

        [TestMethod]
        public void RemoveProductFromBasket()
        {
            marketManagerFacade.AddToCart(token1, 1, productID1, 1);
            marketManagerFacade.RemoveFromCart(token1, 11, 1, 1);
            Client client = clientManager.GetClientByIdentifier(token1);
            Dictionary<int, Basket> baskets = client.Cart.GetBaskets();
            Basket relevantBasket = baskets[1];
            Assert.IsFalse(relevantBasket.products.ContainsKey(productID1), "Expected the product to be removed from the basket.");
        }

        [TestMethod]
        public void AddProductToBasketAndLogout()
        {
            marketManagerFacade.AddToCart(token1, 1, 11, 1);
            Client client = clientManager.GetClientByIdentifier(token1);
            Dictionary<int, Basket> baskets = client.Cart.GetBaskets();
            Basket relevantBasket = baskets[1];
            Assert.IsTrue(relevantBasket.products[productID1] == 1, "Expected the product to be added to the basket.");
            marketManagerFacade.LogoutClient(token1);
            token1 = marketManagerFacade.LoginClient(userName, userPassword);
            client = clientManager.GetClientByIdentifier(token1);
            baskets = client.Cart.GetBaskets();
            relevantBasket = baskets[1];
            Assert.IsTrue(relevantBasket.products[productID1] == 1, "Expected the product to persist in the basket after logout and login.");
        }

        [TestMethod]
        public void PurchaseCartFail_Payment_OrderCancel()
        {
            marketManagerFacade.AddToCart(token1, 1, 11, 1);
            mockPaymentSystem.Setup(pay => pay.Pay(It.IsAny<PaymentDetails>(), It.IsAny<double>())).Returns(-1);
            Assert.ThrowsException<Exception>(() => marketManagerFacade.PurchaseCart(token1, paymentDetails, shippingDetails), "Expected purchase to fail due to payment issue.");
            
            Member client = clientManager.GetMemberByIdentifier(token1);
            Store store = marketManagerFacade.GetStore(1);
            Assert.IsTrue(client.OrderHistory.IsEmpty, "Expected no orders in client history after payment failure.");
            Assert.IsTrue(client.Cart.GetBaskets()[1].products.ContainsKey(productID1), "Expected the product to remain in the cart after payment failure.");
            Assert.IsTrue(store.Products.Count == 1, "Expected the product to remain in the store after payment failure.");
        }

        [TestMethod]
        public void PurchaseCartFail_Shipping_OrderCancel()
        {
            marketManagerFacade.AddToCart(token1, 1, 11, 1);
            mockShippingSystem.Setup(ship => ship.OrderShippment(It.IsAny<ShippingDetails>())).Returns(-1);
            Assert.ThrowsException<Exception>(() => marketManagerFacade.PurchaseCart(token1, paymentDetails, shippingDetails), "Expected purchase to fail due to shipping issue.");
            
            Member client = clientManager.GetMemberByIdentifier(token1);
            Store store = marketManagerFacade.GetStore(1);
            Assert.IsTrue(client.OrderHistory.IsEmpty, "Expected no orders in client history after shipping failure.");
            Assert.IsTrue(client.Cart.GetBaskets()[1].products.ContainsKey(productID1), "Expected the product to remain in the cart after shipping failure.");
            Assert.IsTrue(store.Products.Count == 1, "Expected the product to remain in the store after shipping failure.");
        }

        [TestMethod]
        public void Offline_Notifications_Success()
        {
            marketManagerFacade.NotificationOff(token1);
            marketManagerFacade.AddToCart(token1, 1, productID1, 1);
            marketManagerFacade.PurchaseCart(token1, paymentDetails, shippingDetails);
            Member client = clientManager.GetMemberByIdentifier(token1);
            Assert.IsTrue(client.alerts.Count == 1, "Expected one notification after purchase when offline notifications are enabled.");
        }

        [TestMethod]
        public void Offline_Notifications_Fail_NotOffline()
        {
            marketManagerFacade.AddToCart(token1, 1, productID1, 1);
            marketManagerFacade.PurchaseCart(token1, paymentDetails, shippingDetails);
            Member client = clientManager.GetMemberByIdentifier(token1);
            Assert.IsTrue(client.alerts.Count == 0, "Expected no notifications after purchase when offline notifications are disabled.");
        }

        [TestMethod]
        public void TestPurchaseTransaction(){
            marketManagerFacade.AddToCart(token1, 1, productID1, 1);
            marketManagerFacade.CreateStore(token1, "test", "hadaspr100@gmail.com", "0504457768");
            marketManagerFacade.AddProduct(2, token1, "test", "RegularSell", "nice", 1.0, "fruit", 10, false);
            int rule = marketManagerFacade.AddQuantityRule(token1, 2, "test", 10, 100);
            DateTime expirationDate = DateTime.Now.AddDays(2);
            marketManagerFacade.AddPurchasePolicy(token1, 2, expirationDate, "test", rule);
            marketManagerFacade.AddToCart(token1, 2, 21, 1);
            Assert.ThrowsException<Exception>(() =>marketManagerFacade.PurchaseCart(token1, paymentDetails, shippingDetails));
        }

        // [TestMethod]
        // public void RunMultyTimes()
        // {
        //     for (int i=0; i<5; i++){
        //         AddProductToShop();
        //         Cleanup();
        //         Setup();
        //         RemoveProductFromShop();
        //         Cleanup();
        //         Setup();
        //         AddProductToBasket();
        //         Cleanup();
        //         Setup();
        //         RemoveProductFromBasket();
        //         Cleanup();
        //         Setup();
        //         AddProductToBasketAndLogout();
        //         Cleanup();
        //         Setup();
        //         PurchaseCartFail_Payment_OrderCancel();
        //         Cleanup();
        //         Setup();
        //         PurchaseCartFail_Shipping_OrderCancel();
        //         Cleanup();
        //         Setup();
        //         Offline_Notifications_Success();
        //         Cleanup();
        //         Setup();
        //         Offline_Notifications_Fail_NotOffline();
        //         Cleanup();
        //         Setup();
        //     }
        // }
    }
}

using System.IO.Compression;
using EcommerceAPI.initialize;
using MarketBackend.DAL.DTO;
using MarketBackend.Domain.Market_Client;
using MarketBackend.Domain.Payment;
using MarketBackend.Domain.Shipping;
using MarketBackend.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NLog;

namespace MarketBackend.Tests.AT
{
    [TestClass()]
    public class CorrectnessAT
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
        Proxy proxy;
        int productID1 = 111;
        string productName1 = "Banana";
        string category1 = "Fruit";
        double price1 = 5.0;
        double price2 = 10.0;
        double negPrice = -10.0;
        int quantity1 = 10;
        int quantity2 = 20;
        int negQuantity = -10;
        double discount1 = 0.5;
        double discount2 = 0.3;  
        int userAge = 20;
        int userAge2 = 16;



        [TestInitialize()]
        public void Setup(){
            // DBcontext.SetTestDB();
            proxy = new Proxy();
            userId = proxy.GetUserId();
            // var mockShippingSystem = new Mock<IShippingSystemFacade>();
            // var mockPaymentSystem = new Mock<IPaymentSystemFacade>();
            // mockPaymentSystem.Setup(pay =>pay.Connect()).Returns(true);
            // mockShippingSystem.Setup(ship => ship.Connect()).Returns(true);
            // mockPaymentSystem.Setup(pay =>pay.Pay(It.IsAny<PaymentDetails>(), It.IsAny<double>())).Returns(1);
            // mockShippingSystem.Setup(ship =>ship.OrderShippment(It.IsAny<ShippingDetails>())).Returns(1);
            // mockShippingSystem.SetReturnsDefault(true);
            // mockPaymentSystem.SetReturnsDefault(true);
            RealPaymentSystem paymentSystem = new RealPaymentSystem("https://damp-lynna-wsep-1984852e.koyeb.app/");
            RealShippingSystem shippingSystem = new RealShippingSystem("https://damp-lynna-wsep-1984852e.koyeb.app/");
            new Configurate(MarketService.GetInstance(shippingSystem, paymentSystem), ClientService.GetInstance(shippingSystem, paymentSystem)).Parse("initialize\\configTest.json");
            DBcontext.GetInstance().Dispose();
            proxy.InitiateSystemAdmin();
            proxy.EnterAsGuest(session1);
            proxy.Register(userName, userPassword, email1, userAge);
            token1 = proxy.LoginWithToken(userName, userPassword);
        }

        [TestCleanup]
        public void CleanUp(){
            DBcontext.GetInstance().Dispose();
            proxy.Dispose();
        }
 
        [TestMethod]
        public void UniqueUsername_GoodCase()
        {
            Assert.IsTrue(proxy.EnterAsGuest(session2));
            Assert.IsTrue(proxy.Register(userName2, pass2, email2, userAge2), 
            "Fail in regiter, should not throw exception.");
        }

        [TestMethod]
        public void UniqueUsername_BadCase()
        {
            int userId2 = proxy.GetUserId();
            Assert.IsTrue(proxy.EnterAsGuest(session2));
            Assert.IsFalse(proxy.Register(userName, userPassword, email1, userAge), 
            "Fail in regiter, should throw exception- not unique username.");
        }

        // [TestMethod]
        // public void RunMultyTimes()
        // {
        //     for (int i = 0; i < 5; i ++){
        //         UniqueUsername_GoodCase();
        //         CleanUp();
        //         Setup();
        //         UniqueUsername_BadCase();
        //         CleanUp();
        //         Setup();
        //     }
        // }
    }
}
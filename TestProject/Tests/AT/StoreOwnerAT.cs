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
    public class StoreOwnerAT
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
        int productID1 = 11;
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
        string storeName = "Rami Levi";
        string storeEmail = "RamiLevi@gmail.com";
        string phoneNum  = "0522458976";
        string sellmethod = "RegularSell";
        string desc = "nice";
        int shopID = 1;

        [TestInitialize()]
        public void Setup(){
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
            userId = proxy.GetMembeIDrByUserName(userName);
            proxy.CreateStore(token1, storeName, storeEmail, phoneNum);
        }

        [TestCleanup]
        public void CleanUp(){
            DBcontext.GetInstance().Dispose();
            proxy.Dispose();
        }

        [TestMethod]
        public void AddProductSuccess()
        {
            Assert.IsTrue(proxy.AddProduct(shopID, token1, productName1, sellmethod, desc, price1, category1, quantity1, false));
            Assert.IsTrue(proxy.GetProductInfo(shopID, 11));
        }

        [TestMethod]
        public void RemoveProductSuccess()
        {
            Assert.IsTrue(proxy.AddProduct(shopID, token1, productName1, sellmethod, desc, price1, category1, quantity1, false));
            Assert.IsTrue(proxy.RemoveProduct(shopID, token1, 11));
            Assert.IsFalse(proxy.GetProductInfo(shopID, 11));
        }

        [TestMethod]
        public void RemoveProductFail_NoProduct()
        {
            Assert.IsFalse(proxy.RemoveProduct(shopID, token1, 11));
        }

        [TestMethod]
        public void UpdateProductPriceSuccess()
        {
            Assert.IsTrue(proxy.AddProduct(shopID, token1, productName1, sellmethod, desc, price1, category1, quantity1, false)); 
            Assert.IsTrue(proxy.UpdateProductPrice(shopID, token1, productID1, price2));
            Assert.AreEqual(price2, proxy.GetProduct(shopID, productID1)._price, 
                $"Expected product price to be {price2}, but found {proxy.GetProduct(shopID, productID1)._price}.");     
        }

        [TestMethod]
        public void UpdateProductPriceFail_NegativePrice()
        {
            Assert.IsTrue(proxy.AddProduct(shopID, token1, productName1, sellmethod, desc, price1, category1, quantity1, false)); 
            Assert.IsFalse(proxy.UpdateProductPrice(shopID, token1, productID1, negPrice));
            Assert.AreEqual(price1, proxy.GetProduct(shopID, productID1)._price, 
                $"Expected product price to remain {price1}, but found {proxy.GetProduct(shopID, productID1)._price}.");     
        }

        [TestMethod]
        public void UpdateProductPriceFail_NoProduct()
        {
            Assert.IsFalse(proxy.UpdateProductPrice(shopID, token1, productID1, negPrice));     
        }

        [TestMethod]
        public void UpdateProductQuantitySuccess()
        {
            Assert.IsTrue(proxy.AddProduct(shopID, token1, productName1, sellmethod, desc, price1, category1, quantity1, false)); 
            Assert.IsTrue(proxy.UpdateProductQuantity(shopID, token1, productID1, quantity2));
            Assert.AreEqual(quantity2, proxy.GetProduct(shopID, productID1)._quantity, 
                $"Expected product quantity to be {quantity2}, but found {proxy.GetProduct(shopID, productID1)._quantity}.");      
        }

        [TestMethod]
        public void UpdateProductQuantityFail_NegativeQuantity()
        {
            Assert.IsTrue(proxy.AddProduct(shopID, token1, productName1, sellmethod, desc, price1, category1, quantity1, false)); 
            Assert.IsFalse(proxy.UpdateProductQuantity(shopID, token1, productID1, negQuantity));
            Assert.AreEqual(quantity1, proxy.GetProduct(shopID, productID1)._quantity, 
                $"Expected product quantity to remain {quantity1}, but found {proxy.GetProduct(shopID, productID1)._quantity}.");     
        }

        [TestMethod]
        public void UpdateProductQuantityFail_NoProduct()
        {
            Assert.IsFalse(proxy.UpdateProductQuantity(shopID, token1, productID1, negQuantity));     
        }

        [TestMethod]
        public void GetPurchaseHistorySuccess()
        {
            Assert.IsTrue(proxy.AddProduct(shopID, token1, productName1, sellmethod, desc, price1, category1, quantity1, false));
            int userId2 = proxy.GetUserId();
            Assert.IsTrue(proxy.EnterAsGuest(session2));
            Assert.IsTrue(proxy.Register(userName2, pass2, email2, userAge));
            userId2 = proxy.GetMembeIDrByUserName(userName2);
            token2 = proxy.LoginWithToken(userName2, pass2);
            Assert.IsTrue(proxy.AddToCart(token2, shopID, productID1, quantity1));
            PaymentDetails paymentDetails = new PaymentDetails("ILS", "5326888878675678", "2027", "10", "101", "3190876789", "Hadas");
            ShippingDetails shippingDetails = new ShippingDetails("name",  "city",  "address",  "country",  "zipcode");
            Assert.IsTrue(proxy.PurchaseCart(token2, paymentDetails, shippingDetails));
            Assert.IsTrue(proxy.GetPurchaseHistoryByClient(userName2));
            Assert.AreEqual(1, proxy.GetPurchaseHistory(userName2).Count, 
                $"Expected purchase history count to be 1, but found {proxy.GetPurchaseHistory(userName2).Count}.");
        }

        [TestMethod]
        public void AddStaffMemberSuccess()
        {
            int userId2 = proxy.GetUserId();
            Assert.IsTrue(proxy.EnterAsGuest(session2));
            Assert.IsTrue(proxy.Register(userName2, pass2, email2, userAge));
            userId2 = proxy.GetMembeIDrByUserName(userName2);
            token2 = proxy.LoginWithToken(userName2, pass2);
            Assert.IsTrue(proxy.AddOwner(token1, shopID, userName2));
            Assert.AreEqual(1, proxy.GetOwners(shopID).Count, 
                $"Expected owner count to be 1, but found {proxy.GetOwners(shopID).Count}.");
        }

        [TestMethod]
        public void RemoveStaffMemberSuccess()
        {
            int userId2 = proxy.GetUserId();
            Assert.IsTrue(proxy.EnterAsGuest(session2));
            Assert.IsTrue(proxy.Register(userName2, pass2, email2, userAge));
            userId2 = proxy.GetMembeIDrByUserName(userName2);
            token2 = proxy.LoginWithToken(userName2, pass2);
            Assert.IsTrue(proxy.AddOwner(token1, shopID, userName2));
            Assert.IsTrue(proxy.RemoveStaffMember(shopID, token1, userName2));
            Assert.AreEqual(0, proxy.GetOwners(shopID).Count, 
                $"Expected owner count to be 0, but found {proxy.GetOwners(shopID).Count}.");
        }

        // [TestMethod]
        // public void RunMultyTimes(){
        //     for (int i=0; i<5; i++){
        //         AddProductSuccess();
        //         CleanUp();
        //         Setup();
        //         RemoveProductSuccess();
        //         CleanUp();
        //         Setup();
        //         RemoveProductFail_NoProduct();
        //         CleanUp();
        //         Setup();
        //         UpdateProductPriceSuccess();
        //         CleanUp();
        //         Setup();
        //         UpdateProductPriceFail_NegativePrice();
        //         CleanUp();
        //         Setup();
        //         UpdateProductPriceFail_NoProduct();
        //         CleanUp();
        //         Setup();
        //         UpdateProductQuantitySuccess();
        //         CleanUp();
        //         Setup();
        //         UpdateProductQuantityFail_NegativeQuantity();
        //         CleanUp();
        //         Setup();
        //         UpdateProductQuantityFail_NoProduct();
        //         CleanUp();
        //         Setup();
        //         GetPurchaseHistorySuccess();
        //         CleanUp();
        //         Setup();
        //         AddStaffMemberSuccess();
        //         CleanUp();
        //         Setup();
        //         RemoveStaffMemberSuccess();
        //         CleanUp();
        //         Setup();
        //     }
        // }
    }
}

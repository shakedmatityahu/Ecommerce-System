using EcommerceAPI.initialize;
using MarketBackend.DAL.DTO;
using MarketBackend.Domain.Payment;
using MarketBackend.Domain.Shipping;
using MarketBackend.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;

namespace MarketBackend.Tests.AT
{
    [TestClass()]
    public class UserGuestAT
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
        int userAge = 20;
        int userAge2 = 16;

        [TestInitialize()]
        public void Setup(){
            RealPaymentSystem paymentSystem = new RealPaymentSystem("https://damp-lynna-wsep-1984852e.koyeb.app/");
            RealShippingSystem shippingSystem = new RealShippingSystem("https://damp-lynna-wsep-1984852e.koyeb.app/");
            new Configurate(MarketService.GetInstance(shippingSystem, paymentSystem), ClientService.GetInstance(shippingSystem, paymentSystem)).Parse("initialize\\configTest.json");
            DBcontext.GetInstance().Dispose();
            proxy = new Proxy();
            userId = proxy.GetUserId();
            proxy.InitiateSystemAdmin();
        }

        [TestCleanup]
        public void CleanUp(){
            DBcontext.GetInstance().Dispose();
            proxy.Dispose();
            proxy.ExitGuest(session1);
        }

        [TestMethod]
        public void EnterAsGuestSuccess(){
            Assert.IsFalse(proxy.Login(userName, userPassword), 
                "Expected login to fail for unregistered user.");
            Assert.IsTrue(proxy.Register(userName, userPassword, email1, userAge), 
                "Expected registration to succeed.");
        }

        [TestMethod]
        public void RegisterSuccess(){
            Assert.IsTrue(proxy.EnterAsGuest(session1), 
                "Expected to enter as guest successfully.");
            Assert.IsTrue(proxy.Register(userName, userPassword, email1, userAge), 
                "Expected registration to succeed.");
        }

        [TestMethod]
        public void RegisterFail_RegisterTwice(){
            Assert.IsTrue(proxy.EnterAsGuest(session1), 
                "Expected to enter as guest successfully.");
            Assert.IsTrue(proxy.Register(userName, userPassword, email1, userAge), 
                "Expected first registration to succeed.");
            Assert.IsFalse(proxy.Register(userName, userPassword, email1, userAge), 
                "Expected second registration attempt to fail.");
        }

        [TestMethod]
        public void RegisterFail_WrongEmail(){
            Assert.IsTrue(proxy.EnterAsGuest(session1), 
                "Expected to enter as guest successfully.");
            Assert.IsFalse(proxy.Register(userName, userPassword, wrongEmail, userAge), 
                "Expected registration to fail with wrong email format.");
        }

        [TestMethod]
        public void LoginSuccess(){
            Assert.IsTrue(proxy.EnterAsGuest(session1), 
                "Expected to enter as guest successfully.");
            Assert.IsTrue(proxy.Register(userName, userPassword, email1, userAge), 
                "Expected registration to succeed.");
            Assert.IsTrue(proxy.Login(userName, userPassword), 
                "Expected login to succeed with correct credentials.");
        }

        [TestMethod]
        public void LoginFail_NotRegister(){
            Assert.IsTrue(proxy.EnterAsGuest(session1), 
                "Expected to enter as guest successfully.");
            Assert.IsFalse(proxy.Login(userName, userPassword), 
                "Expected login to fail for unregistered user.");
        }

        [TestMethod]
        public void LoginFail_WrongUserName(){
            Assert.IsTrue(proxy.EnterAsGuest(session1), 
                "Expected to enter as guest successfully.");
            Assert.IsTrue(proxy.Register(userName, userPassword, email1, userAge), 
                "Expected registration to succeed.");
            Assert.IsFalse(proxy.Login(userName2, userPassword), 
                "Expected login to fail with wrong username.");
        }

        [TestMethod]
        public void LoginFail_WrongPassword(){
            Assert.IsTrue(proxy.EnterAsGuest(session1), 
                "Expected to enter as guest successfully.");
            Assert.IsTrue(proxy.Register(userName, userPassword, email1, userAge), 
                "Expected registration to succeed.");
            Assert.IsFalse(proxy.Login(userName, pass2), 
                "Expected login to fail with wrong password.");
        }

        [TestMethod]
        public void LogOutSuccess(){
            Assert.IsTrue(proxy.EnterAsGuest(session1), 
                "Expected to enter as guest successfully.");
            Assert.IsTrue(proxy.Register(userName, userPassword, email1, userAge), 
                "Expected registration to succeed.");
            string token1 = proxy.LoginWithToken(userName, userPassword);
            proxy.GetMembeIDrByUserName(userName);
            Assert.IsTrue(proxy.LogOut(token1), 
                "Expected logout to succeed for logged-in user.");
        }

        [TestMethod]
        public void LogOutFail_NotLoggedIn(){
            Assert.IsTrue(proxy.EnterAsGuest(session1), 
                "Expected to enter as guest successfully.");
            Assert.IsTrue(proxy.Register(userName, userPassword, email1, userAge), 
                "Expected registration to succeed.");
            Assert.IsFalse(proxy.LogOut(session1), 
                "Expected logout to fail for user not logged in.");
        }

        // [TestMethod]
        // public void RunMultyTimes()
        // {
        //     for (int i=0; i<5; i++){
        //         EnterAsGuestSuccess();
        //         CleanUp();
        //         Setup();
        //         RegisterSuccess();
        //         CleanUp();
        //         Setup();
        //         RegisterFail_RegisterTwice();
        //         CleanUp();
        //         Setup();
        //         RegisterFail_WrongEmail();
        //         CleanUp();
        //         Setup();
        //         LoginSuccess();
        //         CleanUp();
        //         Setup();
        //         LoginFail_NotRegister();
        //         CleanUp();
        //         Setup();
        //         LoginFail_WrongUserName();
        //         CleanUp();
        //         Setup();
        //         LoginFail_WrongPassword();
        //         CleanUp();
        //         Setup();
        //         LogOutSuccess();
        //         CleanUp();
        //         Setup();
        //         LogOutFail_NotLoggedIn();
        //         CleanUp();
        //         Setup();
        //     }
        // }
    }
}

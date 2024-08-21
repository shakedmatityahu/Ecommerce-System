using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MarketBackend.DAL.DTO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Mail;
using MarketBackend.Domain.Market_Client;
using MarketBackend.Services;
using MarketBackend.DAL;
using MarketBackend.Domain.Models;
using MarketBackend.Domain.Shipping;
using MarketBackend.Domain.Payment;
using EcommerceAPI;
using EcommerceAPI.initialize;




namespace TestProject.Tests
{
    [TestClass]
    public class TestConfig
    {
        MarketManagerFacade MMF;
        ClientManager CM;
        ClientService CC;
        MarketService s;
        string filePath = Path.Combine("..", "..", "..", "EcommerceAPI", "initialize", "initialState.json");
         [TestInitialize]
        public void Setup()
        {
            UpdateConfigField("Initialize", "Empty");
            UpdateInitFileName("Initialize", "true");
            UpdateInitFileName("InitialState", "initialState.json");
            var mockShippingSystem = new Mock<IShippingSystemFacade>();
            var mockPaymentSystem = new Mock<IPaymentSystemFacade>();
            mockPaymentSystem.Setup(pay =>pay.Connect()).Returns(true);
            mockShippingSystem.Setup(ship => ship.Connect()).Returns(true);
            mockPaymentSystem.Setup(pay =>pay.Pay(It.IsAny<PaymentDetails>(), It.IsAny<double>())).Returns(1);
            mockShippingSystem.Setup(ship =>ship.OrderShippment(It.IsAny<ShippingDetails>())).Returns(1);
            mockShippingSystem.SetReturnsDefault(true);
            mockPaymentSystem.SetReturnsDefault(true);
            s = MarketService.GetInstance(mockShippingSystem.Object, mockPaymentSystem.Object);
            // Task task = Task.Run(() =>
            // {
            //     // Code before disposing the MarketContext instance
            //     DBcontext.GetInstance().Dispose();
            //     // Code after disposing the MarketContext instance
            // });
            // task.Wait();
            CC = ClientService.GetInstance(mockShippingSystem.Object, mockPaymentSystem.Object);
            CM = ClientManager.GetInstance();
            
            MMF = MarketManagerFacade.GetInstance(mockShippingSystem.Object, mockPaymentSystem.Object);           
        }
         [TestCleanup]
        public void Cleanup()
        {
            UpdateConfigField("Initialize", "Empty");
            UpdateInitFileName("Initialize", "false");
            DBcontext.GetInstance().Dispose();
            var mockShippingSystem = new Mock<IShippingSystemFacade>();
            var mockPaymentSystem = new Mock<IPaymentSystemFacade>();
            mockPaymentSystem.Setup(pay =>pay.Connect()).Returns(true);
            mockShippingSystem.Setup(ship => ship.Connect()).Returns(true);
            mockPaymentSystem.Setup(pay =>pay.Pay(It.IsAny<PaymentDetails>(), It.IsAny<double>())).Returns(1);
            mockShippingSystem.Setup(ship =>ship.OrderShippment(It.IsAny<ShippingDetails>())).Returns(1);
            mockShippingSystem.SetReturnsDefault(true);
            mockPaymentSystem.SetReturnsDefault(true);
            MarketService.GetInstance(mockShippingSystem.Object, mockPaymentSystem.Object).Dispose();
            ClientService.GetInstance(mockShippingSystem.Object, mockPaymentSystem.Object).Dispose();
            MarketManagerFacade.Dispose();
            ClientManager.GetInstance().Reset();
        }

        [TestMethod]
        public void checkUsersExits(){
            UpdateInitFileName("Initialize", "true");
            new Configurate(s, CC).Parse("initialize\\configTest.json");
            List<Member> ls = ClientRepositoryRAM.GetInstance().GetAll();
            List<string> listofNames = new List<string>() { "u2", "u3", "u4", "u5", "u6", "u1" };
            foreach (Member memName in ls)
            {
                bool flag = false;                
                foreach (string name in listofNames)
                {
                    if (name == memName.UserName)
                        flag = true;
                }
                if (!flag)
                {
                    Assert.Fail();
                    break;
                }
            }

        }

        [TestMethod]
        public void CheckUsersExistConfigOptions()
        {
            UpdateConfigField("Initialize", "File");
            new Configurate(s, CC).Parse("initialize\\configTest.json");
            
            // Set configTest at "Initialize" : File
            List<Member> ls = ClientRepositoryRAM.GetInstance().GetAll();
            List<string> listOfNames = new List<string>() { "u2", "u3", "u4", "u5", "u6", "u1" };
            foreach (Member memName in ls)
            {
                bool flag = false;                
                foreach (string name in listOfNames)
                {
                    if (name == memName.UserName)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    Assert.Fail($"User {memName.UserName} not found in the expected list.");
                }
            }

            // Set configTest at "Initialize" : DB
            UpdateConfigField("Initialize", "DB");
            new Configurate(s, CC).Parse("initialize\\configTest.json");
            ls = ClientRepositoryRAM.GetInstance().GetAll();
            listOfNames = new List<string>() { "u2", "u3", "u4", "u5", "u6", "u1" };
            foreach (Member memName in ls)
            {
                bool flag = false;                
                foreach (string name in listOfNames)
                {
                    if (name == memName.UserName)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    Assert.Fail($"User {memName.UserName} not found in the expected list.");
                }
            }

            // Set configTest at "Initialize" : Empty
            UpdateConfigField("Initialize", "Empty");
            new Configurate(s, CC).Parse("initialize\\configTest.json");
            ls = ClientRepositoryRAM.GetInstance().GetAll();
            Assert.AreEqual(1, ls.Count, "Expected only one user (system_admin) when initialized as empty.");
        }
        
        [TestMethod]
        public void checkStoreExist(){
            UpdateInitFileName("Initialize", "true");
            UpdateConfigField("Initialize", "File");
            // new SceanarioParser(s,CC).defaultParse().Wait();
            new Configurate(s, CC).Parse("initialize\\configTest.json");
            IEnumerable<Store> ls = StoreRepositoryRAM.GetInstance().getAll();
            bool flag = false;
            foreach (Store store in ls)
            {
                if (store.Name == "s1")
                    flag = true;
                
            }
             if (!flag)
                {
                    Assert.Fail();
                }
            
        }

        //[TestMethod]
        public void checkProductExist(){
            UpdateInitFileName("Initialize", "true");
            new Configurate(s, CC).Parse("initialize\\configTest.json");
            IEnumerable<Product> ls = ProductRepositoryRAM.GetInstance().getAll();
            bool flag = false;
            foreach (Product product in ls)
            {
                if (product.Name == "Bamba")
                    flag = true;
                
            }
             if (!flag)
                {
                    Assert.Fail();
                }
        }
            
        
        [TestMethod]
        public void logInWithExistingUserSeccess(){
            UpdateInitFileName("Initialize", "true");
            new SceanarioParser(s, CC).defaultParse().Wait();
            ClientManager.GetInstance().LoginClient("u6","password");
            var client = ClientRepositoryRAM.GetInstance().GetByUserName("u6");
            Assert.IsTrue(client.IsLoggedIn == true);
            
        }
        [TestMethod]
        public void logInWithExistingUserFail(){
            UpdateInitFileName("Initialize", "true");
            new Configurate(s, CC).Parse("initialize\\configTest.json");
            Assert.ThrowsException<Exception>(()=>CM.LoginClient("u6","21312"));
            
        }
        
        [TestMethod]
        public void WrongPathFail() {
            UpdateInitFileName("Initialize", "true");
            // var exception = Assert.ThrowsException<AggregateException>(() => new SceanarioParser(s, CC).Parse("wrongRoute").Wait());
            // Assert.IsTrue(exception.InnerExceptions.Any(e => e is FileNotFoundException));
            new SceanarioParser(s, CC).Parse("wrongRoute").Wait();
            List<Member> ls = ClientRepositoryRAM.GetInstance().GetAll();
            Assert.AreEqual(ls.Count, 0);
        }

        [TestMethod]
        public void InitSyntaxFail() {
            UpdateInitFileName("Initialize", "true");
            new SceanarioParser(s, CC).Parse("initialize\\initialTestSyntx.json").Wait();
            List<Member> ls = ClientRepositoryRAM.GetInstance().GetAll();
            Assert.AreEqual(ls.Count, 0);
        }

        [TestMethod]
        public void InitValuesFail() {
            UpdateInitFileName("Initialize", "true");
            new SceanarioParser(s, CC).Parse("initialize\\initialTestValues.json").Wait();
            List<Member> ls = ClientRepositoryRAM.GetInstance().GetAll();
            Assert.AreEqual(ls.Count, 0);
        }

        public void UpdateInitFileName(string key, string newValue)
        {
            string filePath = Path.Combine(Environment.CurrentDirectory, "Initialize\\InitialState.json");
            try
            {
                // Read the JSON file
                string jsonString = File.ReadAllText(filePath);

                // Parse the JSON content
                JObject jsonObject = JObject.Parse(jsonString);

                // Find the property with the specified key
                JToken token = jsonObject.SelectToken(key);

                if (token != null)
                {
                    if (newValue.Equals("true"))
                        // Update the value
                        token.Replace(true);
                    else if (newValue.Equals("false"))
                        token.Replace(false);
                    else
                        token.Replace(newValue);
                    // Write the modified JSON back to the file
                    File.WriteAllText(filePath, jsonObject.ToString());
                    Console.WriteLine("Value updated successfully.");
                }
                else
                {
                    Console.WriteLine("Key not found in the JSON file.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        public static void UpdateConfigField(string key, string value)
        {
            string path = "initialize\\configTest.json";
            string json = File.ReadAllText(path);
            JObject jsonObj = JObject.Parse(json);
            jsonObj[key] = value;
            string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            File.WriteAllText(path, output);
        }

        
    }
}
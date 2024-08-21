using Azure.Messaging;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;
using MarketBackend.DAL.DTO;
using MarketBackend.Domain.Market_Client;
using System.IO.Compression;
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
using EcommerceAPI.initialize;

namespace UnitTests
{
    [TestClass]
    public class DBcontextTests
    {
        DBcontext _context = DBcontext.GetInstance();
        private MarketManagerFacade marketManagerFacade;
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
        public void Initialize()
        {
            _context.Dispose();

        }
        [TestCleanup]
        public void CleanUp()
        {
            _context = DBcontext.GetInstance();
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
        }
        [TestMethod]
        public void DisposeTest()
        {
            _context.Dispose();
        }

        [TestMethod]
        public void MarketContextMemberAdd()
        {
            ShoppingCartDTO shoppingCartDTO = new ShoppingCartDTO();
            List<MessageDTO> l = new List<MessageDTO>();
            MemberDTO member1 = new MemberDTO(2, "Hadas", "123", l, false, shoppingCartDTO);
            var add1 = _context.Members.Add(member1);
            _context.SaveChanges();
            Assert.AreEqual(_context.Members.Find(member1.Id), member1);
        }

        [TestMethod]
        public void MarketContextShopAdd()
        {
            ShoppingCartDTO shoppingCartDTO = new ShoppingCartDTO(2);
            List<MessageDTO> l = new List<MessageDTO>();
            MemberDTO member1 = new MemberDTO(2, userName, userPassword, l, false, shoppingCartDTO);
            StoreDTO shop = new StoreDTO(1, storeName, phoneNum, email1, true, 5.0);

            //string name, double price, int quantity, int category, string description, string keywords, List< ReviewDTO > reviews)

            ProductDTO product1 = new ProductDTO(productID1, productName1, price1, quantity1, category1, desc, "Fruits,Melons,Green,Summer Fruits", sellmethod, 1.0);
            ProductDTO product2 = new ProductDTO(12, "Pumpkin", 15, 25, "Vegtables", "Sliced pumpkin, price per 1kg.", "Vegtables,Melons,Orange", sellmethod, 0.5);
            shop.Products.Add(product1);
            shop.Products.Add(product2);

            _context.Stores.Add(shop);
            _context.Members.Add(member1);
            _context.SaveChanges();

            Assert.AreEqual(_context.Stores.Find(shop.Id), shop);
        }

        [TestMethod]
        public void MarketContextAppoints()
        {
            ShoppingCart cart = new ShoppingCart(2);
            ShoppingCartDTO shoppingCartDTO = new ShoppingCartDTO(cart);
            List<MessageDTO> l = new List<MessageDTO>();
            MemberDTO member1 = new MemberDTO(2, userName, userPassword, l, false, shoppingCartDTO);
            ShoppingCart cart2 = new ShoppingCart(3);
            ShoppingCartDTO shoppingCartDTO2 = new ShoppingCartDTO(cart2);
            MemberDTO member2 = new MemberDTO(3, userName2, pass2,l, false, shoppingCartDTO2);

            StoreDTO shop = new StoreDTO(1, storeName, phoneNum, email1, true, 5.0);
            RoleDTO role1 = new RoleDTO(new Role(new StoreManagerRole(RoleName.Manager), new Member(member1), 1, userName2));
            
            ProductDTO product1 = new ProductDTO(productID1, productName1, price1, quantity1, category1, desc, "Fruits,Melons,Green,Summer Fruits", sellmethod, 1.0);
            ProductDTO product2 = new ProductDTO(12, "Pumpkin", 15, 25, "Vegtables", "Sliced pumpkin, price per 1kg.", "Vegtables,Melons,Orange", sellmethod, 0.5);
            shop.Products.Add(product1);
            shop.Products.Add(product2);

            _context.Stores.Add(shop);
            _context.Members.Add(member1);
            _context.SaveChanges();

            _context.Roles.Add(role1);
            _context.SaveChanges();

            Assert.AreEqual(_context.Stores.Find(shop.Id), shop);
        

            RoleDTO queryAppoint = _context.Roles.Find(1, member2.UserName);
            Assert.AreEqual(role1, queryAppoint);

        }

        [TestMethod]
        public void MarketContextAddToBasket()
        {
            ShoppingCart cart = new ShoppingCart(2);
            ShoppingCartDTO shoppingCartDTO = new ShoppingCartDTO(cart);
            List<MessageDTO> l = new List<MessageDTO>();
            MemberDTO member1 = new MemberDTO(2, userName, userPassword, l, false, shoppingCartDTO);
            ShoppingCart cart2 = new ShoppingCart(3);
            ShoppingCartDTO shoppingCartDTO2 = new ShoppingCartDTO(cart2);
            MemberDTO member2 = new MemberDTO(3, userName2, pass2,l, false, shoppingCartDTO2);

            StoreDTO shop = new StoreDTO(1, storeName, phoneNum, email1, true, 5.0);

            ProductDTO product1 = new ProductDTO(productID1, productName1, price1, quantity1, category1, desc, "Fruits,Melons,Green,Summer Fruits", sellmethod, 1.0);
            ProductDTO product2 = new ProductDTO(12, "Pumpkin", 15, 25, "Vegtables", "Sliced pumpkin, price per 1kg.", "Vegtables,Melons,Orange", sellmethod, 0.5);
            shop.Products.Add(product1);
            shop.Products.Add(product2);

            _context.Stores.Add(shop);
            _context.Members.Add(member1);
            _context.Members.Add(member2);
            _context.SaveChanges();

            //int shopId, int shoppingCartId
            BasketDTO basket1 = new BasketDTO(shop.Id, new List<BasketItemDTO>());
            //int id, int productId, double priceAfterDiscount, int quantity
            BasketItemDTO basketItem1 = new BasketItemDTO(product1, 28.77, 900, 10);
            BasketItemDTO basketItem2 = new BasketItemDTO(product2, 10000, 100000, 2);
            basket1.BasketItems.Add(basketItem1);
            basket1.BasketItems.Add(basketItem2);
            member2.ShoppingCart.Baskets.Add(basket1);
            _context.SaveChanges();

            ShoppingCartDTO queryShoppingCart = _context.ShoppingCarts.Find(member2.ShoppingCart._shoppingCartId);
            Assert.AreEqual(basket1, queryShoppingCart.Baskets[0]);

            BasketItemDTO queryBasketItem1 = _context.BasketItems.Find(basketItem1.Id);
            Assert.AreEqual(basketItem1, queryBasketItem1);

            BasketItemDTO queryBasketItem2 = _context.BasketItems.Find(basketItem2.Id);
            Assert.AreEqual(basketItem2, queryBasketItem2);

        }

        [TestMethod]
        public void MarketContextPurchase()
        {
            ShoppingCart cart = new ShoppingCart(2);
            ShoppingCartDTO shoppingCartDTO = new ShoppingCartDTO(cart);
            List<MessageDTO> l = new List<MessageDTO>();
            MemberDTO member1 = new MemberDTO(2, userName, userPassword, l, false, shoppingCartDTO);
            ShoppingCart cart2 = new ShoppingCart(3);
            ShoppingCartDTO shoppingCartDTO2 = new ShoppingCartDTO(cart2);
            MemberDTO member2 = new MemberDTO(3, userName2, pass2,l, false, shoppingCartDTO2);

            StoreDTO shop = new StoreDTO(1, storeName, phoneNum, email1, true, 5.0);
            StoreDTO shop2 = new StoreDTO(2, "store", phoneNum, email1, true, 5.0);

            ProductDTO product1 = new ProductDTO(productID1, productName1, price1, quantity1, category1, desc, "Fruits,Melons,Green,Summer Fruits", sellmethod, 1.0);
            shop.Products.Add(product1);
            ProductDTO product2 = new ProductDTO(12, "Pumpkin", 15, 25, "Vegtables", "Sliced pumpkin, price per 1kg.", "Vegtables,Melons,Orange", sellmethod, 0.5);
            shop.Products.Add(product2);
            ProductDTO product3 = new ProductDTO(13, "Candys", 1, 1000, "sweets", "Candys, price per 1kg.", "Candys,Snacks,Party", sellmethod, 0.2);
            shop2.Products.Add(product3);

            _context.Stores.Add(shop);
            _context.SaveChanges();
            _context.Stores.Add(shop2);
            _context.SaveChanges();
            _context.Members.Add(member1);
            _context.Members.Add(member2);
            _context.SaveChanges();

            BasketDTO basket1 = new BasketDTO(shop.Id, new List<BasketItemDTO>());
            BasketItemDTO basketItem1 = new BasketItemDTO(product1, 28.77, 900, 10);
            BasketItemDTO basketItem2 = new BasketItemDTO(product2, 10000, 100000, 2);
            basket1.BasketItems.Add(basketItem1);
            basket1.BasketItems.Add(basketItem2);
            member1.ShoppingCart.Baskets.Add(basket1);
            _context.SaveChanges();


            ShoppingCartDTO queryShoppingCart = _context.ShoppingCarts.Find(member1.ShoppingCart._shoppingCartId);
            Assert.AreEqual(1, queryShoppingCart.Baskets.Count);
            Assert.AreEqual(basket1, queryShoppingCart.Baskets[0]);

            BasketItemDTO queryBasketItem1 = _context.Find<BasketItemDTO>(basketItem1.Id);
            Assert.AreEqual(basketItem1, queryBasketItem1);

            BasketItemDTO queryBasketItem2 = _context.Find<BasketItemDTO>(basketItem2.Id);
            Assert.AreEqual(basketItem2, queryBasketItem2);

            PurchaseDTO purchase1 = new PurchaseDTO(1, shop.Id, basket1, session1, basketItem1.PriceAfterDiscount + basketItem2.PriceAfterDiscount);

            shop.Purchases.Add(purchase1);

            _context.SaveChanges();

            PurchaseDTO purchaseDTO1 = _context.Find<PurchaseDTO>(purchase1.Id);
            Assert.AreEqual(purchaseDTO1, purchase1);
        }

        // [TestMethod]
        // public void MarketContextAddPoliciesAndRules()
        // {
        //     //make shop to add the rules and policies to
        //     ShoppingCart cart = new ShoppingCart(1);
        //     ShoppingCartDTO shoppingCartDTO = new ShoppingCartDTO(cart);
        //     List<MessageDTO> l = new List<MessageDTO>();
        //     MemberDTO member1 = new MemberDTO(1, userName, userPassword, l, false, shoppingCartDTO);
        //     StoreDTO shop = new StoreDTO(1, storeName, phoneNum, email1, true, 5.0);

        //     ProductDTO product1 = new ProductDTO(productID1, productName1, price1, quantity1, category1, desc, "Fruits,Melons,Green,Summer Fruits", sellmethod, 1.0);
        //     shop.Products.Add(product1);
        //     ProductDTO product2 = new ProductDTO(12, "Pumpkin", 15, 25, "Vegtables", "Sliced pumpkin, price per 1kg.", "Vegtables,Melons,Orange", sellmethod, 0.5);
        //     shop.Products.Add(product2);

        //     _context.Stores.Add(shop);
        //     _context.Members.Add(member1);
        //     _context.SaveChanges();

        //     //just to make sure the shop is in the DB
        //     Assert.AreEqual(_context.Stores.Find(shop.Id), shop);

        //     ProductDTO emptyProduct = new ProductDTO(-1, "Dummy Product", 0, 0, "Non", "Do not use!", "");
        //     RuleSubject ruleSubjet1 = new RuleSubject(new Product(product1));
        //     RuleSubject ruleSubjet2 = new RuleSubject("Fruits");
        //     RuleSubject ruleSubjet3 = new RuleSubject(new Product(product2));


        //     PolicySubjectDTO policySubjet1 = new PolicySubjectDTO(product1, "null");
        //     PolicySubjectDTO policySubjet2 = new PolicySubjectDTO(emptyProduct, "Vegtables");
        //     PolicySubjectDTO policySubjet3 = new PolicySubjectDTO(emptyProduct, "Food");

        //     QuantityRule qrule1 = new QuantityRule(1, shop.Id, ruleSubjet1, 10, 1000);
        //     QuantityRuleDTO quantityRule1 = new QuantityRuleDTO(qrule1);
        //     QuantityRule qrule2 = new QuantityRule(2, shop.Id, ruleSubjet2, 1, 1000);
        //     QuantityRuleDTO quantityRule2 = new QuantityRuleDTO(qrule2);
        //     TotalPriceRule trule = new TotalPriceRule(3, shop.Id, ruleSubjet2, 200);
        //     TotalPriceRuleDTO totalPriceRule = new TotalPriceRuleDTO(trule);
        //     SimpleRule srule = new SimpleRule(4, shop.Id, ruleSubjet1);
        //     SimpleRuleDTO simpleRule = new SimpleRuleDTO(srule);
        //     CompositeRule crule = new CompositeRule(5, shop.Id, new List<IRule> { qrule1, qrule2 }, LogicalOperator.Xor); 
        //     CompositeRuleDTO compositeRule = new CompositeRuleDTO(crule);

        //     shop.Rules.Add(quantityRule1);
        //     shop.Rules.Add(quantityRule2);
        //     shop.Rules.Add(totalPriceRule);
        //     shop.Rules.Add(simpleRule);
        //     _context.SaveChanges();

        //     shop.Rules.Add(compositeRule);

        //     _context.SaveChanges();

        //     RuleDTO queryRule1 = _context.Rules.Find(quantityRule1.Id);
        //     Assert.AreEqual(quantityRule1, queryRule1);

        //     RuleDTO queryRule2 = _context.Rules.Find(quantityRule2.Id);
        //     Assert.AreEqual(quantityRule2, queryRule2);

        //     RuleDTO querytotalPriceRule = _context.Rules.Find(totalPriceRule.Id);
        //     Assert.AreEqual(totalPriceRule, querytotalPriceRule);

        //     RuleDTO querySimpleRule = _context.Rules.Find(simpleRule.Id);
        //     Assert.AreEqual(simpleRule, querySimpleRule);

        //     RuleDTO queryCompositeRule = _context.Rules.Find(compositeRule.Id);
        //     Assert.AreEqual(compositeRule, queryCompositeRule);

        //     //just to make sure the shop is in the DB
        //     Assert.AreEqual(_context.Stores.Find(shop.Id), shop);

        //     PurchasePolicyDTO purPolicy1 = new PurchasePolicyDTO(11, DateTime.Today.AddDays(60), totalPriceRule.Id, policySubjet2);
        //     PurchasePolicyDTO purPolicy2 = new PurchasePolicyDTO(12, DateTime.Today.AddDays(60), quantityRule1.Id, policySubjet3);
        //     //to add to the shop
        //     DiscountPolicyDTO disPolicy = new DiscountPolicyDTO(13, DateTime.Today.AddDays(300), simpleRule.Id, policySubjet1, 10);
        //     DiscountCompositePolicyDTO disCompPolicy = new DiscountCompositePolicyDTO(14, DateTime.Today.AddDays(60), simpleRule.Id, policySubjet3, 25, "Add", new List<PolicyDTO> { purPolicy1, purPolicy2 });
        //     shop.Policies.Add(purPolicy1);
        //     shop.Policies.Add(purPolicy2);
        //     shop.Policies.Add(disPolicy);
        //     _context.SaveChanges();

        //     shop.Policies.Add(disCompPolicy);

        //     _context.SaveChanges();

        //     PolicyDTO queryPurPolicy1 = _context.Policies.Find(purPolicy1.Id);
        //     Assert.AreEqual(purPolicy1, queryPurPolicy1);

        //     PolicyDTO queryPurPoliciy2 = _context.Policies.Find(purPolicy2.Id);
        //     Assert.AreEqual(purPolicy2, queryPurPoliciy2);

        //     PolicyDTO queryDisPolicy = _context.Policies.Find(disPolicy.Id);
        //     Assert.AreEqual(disPolicy, queryDisPolicy);

        //     PolicyDTO queryDisCompPolicy = _context.Policies.Find(disCompPolicy.Id);
        //     Assert.AreEqual(disCompPolicy, queryDisCompPolicy);

        //     //just to make sure the shop is in the DB
        //     Assert.AreEqual(_context.Stores.Find(shop.Id), shop);
        // }

        [TestMethod]
        public void MarketContextEvents()
        {
            ShoppingCart cart = new ShoppingCart(2);
            ShoppingCartDTO shoppingCartDTO = new ShoppingCartDTO(cart);
            List<MessageDTO> l = new List<MessageDTO>();
            MemberDTO member1 = new MemberDTO(2, userName, userPassword, l, false, shoppingCartDTO);
            ShoppingCart cart2 = new ShoppingCart(3);
            ShoppingCartDTO shoppingCartDTO2 = new ShoppingCartDTO(cart2);
            MemberDTO member2 = new MemberDTO(3, userName2, pass2,l, false, shoppingCartDTO2);
            ShoppingCart cart3 = new ShoppingCart(4);
            ShoppingCartDTO shoppingCartDTO3 = new ShoppingCartDTO(cart3);
            MemberDTO member3 = new MemberDTO(4, "Ben", "111", l, true, shoppingCartDTO3);

            StoreDTO shop = new StoreDTO(1, storeName, phoneNum, email1, true, 5.0);
            StoreDTO shop2 = new StoreDTO(2, "Hadas's Shop", phoneNum, email1, true, 5.0);

            _context.Members.Add(member1);
            _context.Members.Add(member2);
            _context.Members.Add(member3);

            _context.Stores.Add(shop);
            _context.Stores.Add(shop2);

            _context.SaveChanges();

            string eventName1 = "Add Appointment Event";
            string eventName2 = "Product Sell Event";
            string eventName3 = "Report Event";

            EventDTO event1 = new EventDTO(eventName1, shop.Id, member1);
            EventDTO event2 = new EventDTO(eventName2, shop.Id, member1);

            EventDTO event3 = new EventDTO(eventName3, shop.Id, member1);
            EventDTO event4 = new EventDTO(eventName3, shop.Id, member2);

            _context.Events.Add(event1);
            _context.Events.Add(event2);
            _context.Events.Add(event3);
            _context.Events.Add(event4);
            _context.SaveChanges();

            List<MemberDTO> listeners1 = DBcontext.GetInstance().Events.Where(e => e.StoreId == shop.Id & e.Name.ToLower().Equals(eventName1.ToLower())).Select(e => e.Listener).ToList();
            Assert.AreEqual(1, listeners1.Count);
            Assert.AreEqual(listeners1[0].Id, member1.Id);

            List<MemberDTO> listeners2 = DBcontext.GetInstance().Events.Where(e => e.StoreId == shop.Id & e.Name.ToLower().Equals(eventName2.ToLower())).Select(e => e.Listener).ToList();
            Assert.AreEqual(1, listeners2.Count);
            Assert.AreEqual(listeners2[0].Id, member1.Id);


            List<MemberDTO> listeners3 = DBcontext.GetInstance().Events.Where(e => e.StoreId == shop.Id & e.Name.ToLower().Equals(eventName3.ToLower())).Select(e => e.Listener).ToList();
            Assert.AreEqual(2, listeners3.Count);
            Assert.AreEqual(listeners3[0].Id, member1.Id);
            Assert.AreEqual(listeners3[1].Id, member2.Id);

        }
        
    }
}
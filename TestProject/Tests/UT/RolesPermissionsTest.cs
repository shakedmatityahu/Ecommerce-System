using MarketBackend.DAL.DTO;
using MarketBackend.Domain.Market_Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketBackend.DAL;
using MarketBackend.DAL.DTO;
using MarketBackend.Tests.AT;
using MarketBackend.Services;
using MarketBackend.Domain.Shipping;
using MarketBackend.Domain.Payment;
using Moq;
using EcommerceAPI.initialize;

namespace UnitTests
{
    [TestClass]
    public class RolesPermissionsTest
    {
        Role founder;
        Role owner;
        Role manager;
        string username0 = "username0";
        string username1 = "username1";
        string username2 = "username2"; 
        RoleRepositoryRAM rrr = RoleRepositoryRAM.GetInstance();


        [TestInitialize]
        public void SetUp()
        {
            var mockShippingSystem = new Mock<IShippingSystemFacade>();
            var mockPaymentSystem = new Mock<IPaymentSystemFacade>();
            mockPaymentSystem.Setup(pay =>pay.Connect()).Returns(true);
            mockShippingSystem.Setup(ship => ship.Connect()).Returns(true);
            mockPaymentSystem.Setup(pay =>pay.Pay(It.IsAny<PaymentDetails>(), It.IsAny<double>())).Returns(1);
            mockShippingSystem.Setup(ship =>ship.OrderShippment(It.IsAny<ShippingDetails>())).Returns(1);
            mockShippingSystem.SetReturnsDefault(true);
            mockPaymentSystem.SetReturnsDefault(true);
            DBcontext.SetTestDB();
            DBcontext.GetInstance().Dispose();
            MarketService s = MarketService.GetInstance(mockShippingSystem.Object, mockPaymentSystem.Object);
            ClientService c = ClientService.GetInstance(mockShippingSystem.Object, mockPaymentSystem.Object);
            ClientManager CM = ClientManager.GetInstance();
            MarketManagerFacade MMF = MarketManagerFacade.GetInstance(mockShippingSystem.Object, mockPaymentSystem.Object);
            c.Register(username0, "12345", "nofar@gmail.com", 19);
            c.Register(username1, "12345", "nofar1@gmail.com", 20);
            c.Register(username2, "12345", "nofar2@gmail.com", 21);
            string token1 = c.LoginClient(username0, "12345").Value;
            int storeId = MMF.CreateStore(token1, "shop1", "shop@gmail.com", "0502552798");
            Store store = MMF.GetStore(storeId);
            founder = store.getRole(username0);
            owner = new Role(new Owner(RoleName.Owner), MMF.GetMember(username0), storeId, username1);
            store.AddStaffMember(username1, owner, username0);
            manager = new Role(new StoreManagerRole(RoleName.Manager), MMF.GetMember(username0), storeId, username2);
            store.AddStaffMember(username2, manager, username0);
        }

        [TestCleanup]
        public void CleanUp()
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
            MarketService.GetInstance(mockShippingSystem.Object, mockPaymentSystem.Object).Dispose();
            ClientService.GetInstance(mockShippingSystem.Object, mockPaymentSystem.Object).Dispose();
            ClientManager.Dispose();
            MarketManagerFacade.Dispose();
        }

        [TestMethod]
        public void canAddProductSuccess()
        {
            manager.addPermission(Permission.addProduct);
            rrr.Update(manager);
            Assert.IsTrue(manager.canAddProduct());

            Assert.IsTrue(founder.canAddProduct());
            Assert.IsTrue(owner.canAddProduct());
        }

        [TestMethod]
        public void canAddProductFailure()
        {
            manager.addPermission(Permission.addProduct);
            rrr.Update(manager);
            manager.removePermission(Permission.addProduct);
            rrr.Update(manager);
            Assert.IsFalse(manager.canAddProduct());
        }

        [TestMethod]
        public void canRemoveProductSuccess()
        {
            manager.addPermission(Permission.removeProduct);
            rrr.Update(manager);
            Assert.IsTrue(manager.canRemoveProduct());

            Assert.IsTrue(owner.canRemoveProduct());
            Assert.IsTrue(founder.canRemoveProduct());
        }

        [TestMethod]
        public void canRemoveProductFailure()
        {
            manager.addPermission(Permission.removeProduct);
            rrr.Update(manager);
            manager.removePermission(Permission.removeProduct);
            rrr.Update(manager);
            Assert.IsFalse(manager.canRemoveProduct());
        }

        [TestMethod]
        public void canUpdateProductPriceSuccess()
        {
            manager.addPermission(Permission.updateProductPrice);
            rrr.Update(manager);
            Assert.IsTrue(manager.canUpdateProductPrice());

            Assert.IsTrue(founder.canUpdateProductPrice());
            Assert.IsTrue(owner.canUpdateProductPrice());
        }

        [TestMethod]
        public void canUpdateProductPriceFailure()
        {
            manager.addPermission(Permission.updateProductPrice);
            rrr.Update(manager);
            manager.removePermission(Permission.updateProductPrice);
            rrr.Update(manager);
            Assert.IsFalse(manager.canUpdateProductPrice());
        }

        [TestMethod]
        public void canUpdateProductDiscountSuccess()
        {
            manager.addPermission(Permission.updateProductDiscount);
            rrr.Update(manager);
            Assert.IsTrue(manager.canUpdateProductDiscount());

            Assert.IsTrue(founder.canUpdateProductDiscount());
            Assert.IsTrue(owner.canUpdateProductDiscount());
        }

        [TestMethod]
        public void canUpdateProductDiscountFailure()
        {
            manager.addPermission(Permission.updateProductDiscount);
            rrr.Update(manager);
            manager.removePermission(Permission.updateProductDiscount);
            rrr.Update(manager);
            Assert.IsFalse(manager.canUpdateProductDiscount());
        }

        [TestMethod]
        public void canUpdateProductQuantitySuccess()
        {
            manager.addPermission(Permission.updateProductQuantity);
            rrr.Update(manager);
            Assert.IsTrue(manager.canUpdateProductQuantity());

            Assert.IsTrue(founder.canUpdateProductQuantity());
            Assert.IsTrue(owner.canUpdateProductQuantity());
        }

        [TestMethod]
        public void canUpdateProductQuantityFailure()
        {
            manager.addPermission(Permission.updateProductQuantity);
            rrr.Update(manager);
            manager.removePermission(Permission.updateProductQuantity);
            rrr.Update(manager);
            Assert.IsFalse(manager.canUpdateProductQuantity());
        }

        [TestMethod]
        public void canCloseStoreTest()
        {
            Assert.IsFalse(manager.canCloseStore());
            Assert.IsFalse(owner.canCloseStore());
            Assert.IsTrue(founder.canCloseStore());
        }

        [TestMethod]
        public void canOpenStoreTest()
        {
            Assert.IsFalse(manager.canOpenStore());
            Assert.IsTrue(owner.canOpenStore());
            Assert.IsTrue(founder.canOpenStore());
        }

        [TestMethod]
        public void canAddStaffMemberManagerTest()
        {
            Assert.IsFalse(manager.canAddStaffMember(RoleName.Manager));
            Assert.IsFalse(manager.canAddStaffMember(RoleName.Owner));
            Assert.IsFalse(manager.canAddStaffMember(RoleName.Founder));
        }

        [TestMethod]
        public void canAddStaffMemberOwnerTest()
        {
            Assert.IsTrue(owner.canAddStaffMember(RoleName.Manager));
            Assert.IsTrue(owner.canAddStaffMember(RoleName.Owner));
            Assert.IsFalse(owner.canAddStaffMember(RoleName.Founder));
        }

        [TestMethod]
        public void canAddStaffMemberFounderTest()
        {
            Assert.IsTrue(founder.canAddStaffMember(RoleName.Manager));
            Assert.IsTrue(founder.canAddStaffMember(RoleName.Owner));
            Assert.IsFalse(founder.canAddStaffMember(RoleName.Founder));
        }

    }
}

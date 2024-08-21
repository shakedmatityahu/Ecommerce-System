using MarketBackend.Domain.Payment;
using Moq;

namespace UnitTests
{
    [TestClass]
    public class PaymentSystemsTest
    {
        string cardNumber = "2222333344445555";
        string exprYear = "2027";

        string currency = "ILS";
        string exprMonth = "01";
        string cvv = "123";
        string cardID = "206971997";
        string name = "Shaked Matityahu";
        double totalAmount = 100.00;

        private IPaymentSystemFacade paymentSystem;

        private PaymentDetails paymentDetails;

        private PaymentDetails bad_paymentDetails;

        [TestInitialize]
        public void SetUp()
        {
            paymentSystem = new RealPaymentSystem("https://damp-lynna-wsep-1984852e.koyeb.app/");
            paymentDetails = new PaymentDetails(currency, cardNumber, exprYear, exprMonth, cvv, cardID, name);
            bad_paymentDetails = new PaymentDetails(currency, cardNumber, exprYear, exprMonth, "986", cardID, name);
            
        }

        [TestMethod]
        // Success - Test that the payment system can connect
        public void TestAttemptToConnect()
        {
            Assert.IsTrue(paymentSystem.Connect());
        }

        [TestMethod]
        // Success - Test that the payment system returns a transaction ID when the payment is successful
        public void TestAttemptPaymentSuccess()
        {
            int transactionId = paymentSystem.Pay(paymentDetails, totalAmount);
            Console.WriteLine($"Transaction ID: {transactionId}");  
            Assert.IsTrue(transactionId > 0);
        }

        [TestMethod]
        // Success - Test that the payment system returns -1 when the payment fails due to a missing connection
        public void TestRequestRefundSuccess()
        {
            int transactionId = paymentSystem.Pay(paymentDetails, totalAmount);
            Assert.AreEqual(1, paymentSystem.CancelPayment(transactionId),
            $"Expected 1 but got {paymentSystem.CancelPayment(transactionId)}");
        }

         [TestMethod]
        // Success - Test that the payment system returns -1 when the payment fails due to a missing connection
        public void TestPaymentWithBadCvv()
        {
            int transactionId = paymentSystem.Pay(bad_paymentDetails, totalAmount);
            Assert.AreEqual(-1, transactionId,
            $"Expected -1 but got {transactionId}");
        }

        [TestMethod]
        // Fails - Test with Mock that the payment system returns -1 when the payment fails due to a missing contact
        public void TestMockedPaymentSystemRejection()
        {

            var mockRealShippingSystem = new Mock<RealPaymentSystem>("https://damp-lynna-wsep-1984852e.koyeb.app/");
            mockRealShippingSystem.Setup(rs => rs.Pay(It.IsAny<PaymentDetails>(), totalAmount)).Returns(-1);

            int transactionId = mockRealShippingSystem.Object.Pay(paymentDetails, totalAmount);
            Assert.AreEqual(-1, transactionId,
            $"Expected fail with -1 but got {transactionId}");
        }

        [TestMethod]
        // Fails - Test that the payment system returns -1 when the payment fails due to wrong total amount
        public void TestPaymentWithWrongTotalAmount()
        {
            totalAmount = -100.00;
            Assert.AreEqual(-1, paymentSystem.Pay(paymentDetails, totalAmount),
            $"Expected fail with -1 but got {paymentSystem.Pay(paymentDetails, totalAmount)}");
        }
 
    }
}
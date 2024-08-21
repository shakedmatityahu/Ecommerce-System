using MarketBackend.DAL.DTO;
using MarketBackend.Domain.Payment;
using MarketBackend.Domain.Security;
using Moq;
using System.Transactions;
using System.Xml.Linq;

namespace UnitTests
{
    [TestClass]
    public class SecuritySystemTest
    {
        private SecurityManager _securityManager;
        private TokenManager _tokenManager;

        string mockUsername = "oli";
        string mockPassword = "olassecurepassword";
        string mockToken;
        DateTime mockIssueTime;
        DateTime mockExpirationTime;

        [TestInitialize]
        public void SetUp()
        {
            _securityManager = SecurityManager.GetInstance();
            _tokenManager = TokenManager.GetInstance();
            mockIssueTime = DateTime.UtcNow;
            mockExpirationTime = mockIssueTime.AddMinutes(_tokenManager.ExpirationTime);
            mockToken = _securityManager.GenerateToken(mockUsername);
        }

        [TestCleanup]
        public void Cleanup()
        {
            DBcontext.GetInstance().Dispose();
        }
        
        [TestMethod]
        public void TestAttemptValidateToken()
        {
            Assert.IsTrue(_securityManager.ValidateToken(mockToken));
        }


        [TestMethod]
        public void TestAttemptExtractUserId() 
        {
            Assert.AreEqual(mockUsername, _securityManager.ExtractUsername(mockToken),
            $"Expected name to be {mockUsername} but got {_securityManager.ExtractUsername(mockToken)}");
        }


        [TestMethod]
        public void TestAttemptExtractIssuedAt()
        {
            var marginOfError = TimeSpan.FromSeconds(1);
            DateTime issuedAt = _securityManager.ExtractIssuedAt(mockToken);
            Assert.IsTrue(issuedAt >= mockIssueTime - marginOfError && issuedAt <= mockIssueTime + marginOfError);
        }

        [TestMethod]
        public void TestAttemptExtractExpiration()
        {
            var marginOfError = TimeSpan.FromSeconds(1);
            DateTime expirationTime = _securityManager.ExtractExpiration(mockToken);
            Assert.IsTrue(expirationTime >= mockExpirationTime - marginOfError && expirationTime <= mockExpirationTime + marginOfError);
        }

        [TestMethod]
        public void TestVerifyPassword()
        {
            string hashedPassword = _securityManager.EncryptPassword(mockPassword);
            Assert.IsTrue(_securityManager.VerifyPassword(mockPassword, hashedPassword));
        }
    }
}

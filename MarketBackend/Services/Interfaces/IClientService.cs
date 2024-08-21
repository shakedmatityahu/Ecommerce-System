using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketBackend.Domain.Market_Client;
using MarketBackend.Domain.Models;
using MarketBackend.Domain.Payment;
using MarketBackend.Services.Models;

namespace MarketBackend.Services.Interfaces
{
    public interface IClientService
    {
        public Response Register(string username, string password, string email, int age);
        public Response<string> EnterAsGuest(string identifier);
        public Response<int> CreateStore(string identifier, string storeName, string email, string phoneNum);
        public Response<bool> ResToStoreManageReq(string identifier);
        public Response<bool> ResToStoreOwnershipReq(string identifier); //respond to store ownership request
        public Response LogoutClient(string identifier);
        public Response RemoveFromCart(string identifier, int productId, int storeId, int quantity);
        public Response<ShoppingCartResultDto> ViewCart(string identifier);
        public Response AddToCart(string identifier, int storeId, int productId, int quantity);
        public Response<string> LoginClient(string username, string password);
        public Response ExitGuest(string identifier);
        public Response<List<ShoppingCartResultDto>> GetPurchaseHistoryByClient(string userName);
        public Response EditPurchasePolicy(int storeId);
        public Response<List<StoreResultDto>> GetMemberStores(string identifier);
        public Response<StoreResultDto> GetMemberStore(string identifier, int storeId);
        public Response<List<MessageResultDto>> GetMemberNotifications(string identifier);
        public Response SetMemberNotifications(string identifier, bool on);
        public Response<string> GetTokenByUserName(string userName);
    }
}

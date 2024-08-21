using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Xml;
using MarketBackend.DAL;
using MarketBackend.Domain.Models;
using MarketBackend.Domain.Security;
using MarketBackend.Services.Interfaces;
using Microsoft.VisualBasic;

namespace MarketBackend.Domain.Market_Client
{
    public class ClientManager
    {
        private static ClientManager Manager = null;
        private static ConcurrentDictionary<string, Member> MemberByToken {get; set;}
        private static ConcurrentDictionary<string, Guest> GuestBySession {get; set;}        
        private readonly IClientRepository _clientRepository;
        private readonly SecurityManager _security;
        public ClientManager(int userCounter) 
        {
            this.UserCounter = userCounter;
   
        }
        private int UserCounter {get; set;}
        private object _lock = new object();
    
        private ClientManager()
        {
            lock(_lock){
                UserCounter = ClientRepositoryRAM.GetInstance().getAll()?.Select(client => client.Id).DefaultIfEmpty(0).Max() + 1 ?? 1;
            }
                        // UserCounter = 1;

            GuestBySession = new ConcurrentDictionary<string, Guest>();
            MemberByToken = new ConcurrentDictionary<string, Member>();
            _clientRepository = ClientRepositoryRAM.GetInstance();
            _security = SecurityManager.GetInstance();
        }        

        public static ClientManager GetInstance()
        {
            Manager ??= new ClientManager();
            return Manager;
        }

        public static void Dispose()
        {
            Manager = null;

        }

        public void Reset()
        {
            lock(_lock){
                UserCounter = 1;
            }
        }

        public static bool CheckClientIdentifier(string identifier)
        {
            if (MemberByToken.ContainsKey(identifier) || GuestBySession.ContainsKey(identifier))
                return true;

            throw new KeyNotFoundException($"Client ID {identifier} not found in members or active guests.");
        }

        public Client GetClientByIdentifier(string identifier)
        {
            if (MemberByToken.TryGetValue(identifier, out var member))
            {
                return member;
            }

            if (GuestBySession.TryGetValue(identifier, out var guest))
            {
                return guest;
            }

            return null;
        }
        public Guest GetGuestByIdentifier(string identiifer)
        {
            if (GuestBySession.TryGetValue(identiifer, out var guest))
            {
                return guest;
            }

            return null;
        }

        public Member GetMemberByIdentifier(string identifier)
        {
            if (MemberByToken.TryGetValue(identifier, out var member))
            {
                return member;
            }            
            return null;
        }

        public bool AddToCart(string identifier, int storeId, int productId, int quantity)
        {
            Client client = GetClientByIdentifier(identifier);
            client?.AddToCart(storeId ,productId, quantity);
            return client is not null;
        }

        public Client Register(string username, string password, string email, int age)
        {
            try
            {
                var newClient = CreateMember(username, password, email, age);
                _clientRepository.Add(newClient);
                return newClient;
            }
            catch (ArgumentException)
            {
                throw;
            }
        }

        private Member CreateMember(string username, string password, string email, int age)
        {
            var emailParsed = ValidateEmail(email);
            ValidateUserName(username);
            ValidatePassword(password);
            
            var newMember = new Member(UserCounter, username, emailParsed, _security.EncryptPassword(password))
            {
                IsAbove18 = age >= 18
            };
            UserCounter++;
            return newMember;
        }


        private MailAddress ValidateEmail(string email){
            try{
                return new MailAddress(email);                
            }
            catch (FormatException){
                throw new ArgumentException("Email address is not valid.");
            }
            
        }

        private bool ValidatePassword(string password){
            if (string.IsNullOrWhiteSpace(password) || password.Contains(' '))
            {
                throw new ArgumentException("Password cannot be empty or contain spaces.", nameof(password));
            }
            return true;            
        }

        private bool ValidateUserName(string username){
            if(_clientRepository.ContainsUserName(username))
            {
                throw new ArgumentException("Username already exists.", nameof(username));
            }
            return true;            
        }

        public void RemoveFromCart(string identifier, int productId, int storeId, int quantity)
        {
            var client = GetClientByIdentifier(identifier);
            client.RemoveFromCart(storeId, productId, quantity);
        }

        public ShoppingCart ViewCart(string identifier)
        {
            var client = GetClientByIdentifier(identifier);
            return client.Cart;
        }

        public bool IsMember(string userName){
            return _clientRepository.ContainsUserName(userName);
        }

        public Member GetSystemAdmin()
        {
            var allMembers = _clientRepository.getAll();
            var allPossibleAdmin = allMembers.Where(member => member.IsSystemAdmin);
            return allPossibleAdmin.FirstOrDefault();
        }

        public Client RegisterAsSystemAdmin(string username, string password, string email, int age)
        {
            var registerAdmin = GetSystemAdmin();
            if (registerAdmin != null) return registerAdmin;

            try
            {
                registerAdmin = CreateMember(username, password, email, age);
                registerAdmin.IsSystemAdmin = true;
                _clientRepository.Add(registerAdmin);
                return registerAdmin;
            }
            catch (ArgumentException)
            {
                throw;
            }
        }

        public string LoginClient(string username, string password)
        {
            try{
                var client = _clientRepository.GetByUserName(username);
                
                if(_security.VerifyPassword(password, client.Password) && !client.IsLoggedIn){
                    var token = _security.GenerateToken(username);
                    MemberByToken.TryAdd(token, client);
                    client.IsLoggedIn = true;
                    return token;
                }
                else
                    throw new Exception(@$"{client.UserName} already logged in.");
                    
            }catch(Exception){
                throw;
            }
        }

        public void LogoutClient(string identifier)
        {
            try{
                if (_security.ValidateToken(identifier))
                {
                    var client = GetMemberByIdentifier(identifier);

                    if (client.IsLoggedIn)
                    {
                        client.IsLoggedIn = false;
                        MemberByToken.TryRemove(new(identifier, client));
                    }
                    else
                    {
                        throw new Exception($"{client.UserName} not logged in");
                    }
                }
                else
                {
                    throw new Exception($"Invalid token");
                }
                
                    
            }catch(Exception){
                throw;
            }
        }

        public void BrowseAsGuest(string identifier)
        {
            var guest = new Guest(UserCounter);
            GuestBySession.TryAdd(identifier, guest);
            UserCounter++;
        }

        public void DeactivateGuest(string identifier)
        {
            var client = GetGuestByIdentifier(identifier);
            GuestBySession.TryRemove(identifier, out client);
        }


        public int GetMemberIDrByUserName(string userName)
        {
            if(_clientRepository.ContainsUserName(userName))
            {
                return _clientRepository.GetByUserName(userName).Id;
            }
            return -1;       
        }

        public Member GetMember(string userName)
        {
            if(_clientRepository.ContainsUserName(userName))
            {
                return _clientRepository.GetByUserName(userName);
            }
            return null;       
        }

        public Member GetMemberByUserName(string userName)
        {
            return _clientRepository.GetByUserName(userName);    
        }

        public bool CheckMemberIsLoggedIn(string identifier)
        {
            if (MemberByToken.TryGetValue(identifier, out var member))
            {
                return member.IsLoggedIn;
            }            
            throw new KeyNotFoundException($"identifier= {identifier} not found in members");
        }

        public List<ShoppingCartHistory> GetPurchaseHistoryByClient(string userName)
        {
            return GetMemberByUserName(userName).GetHistory();
        }

        public void PurchaseBasket(string identifier, Basket basket)
        {
            if(GetMemberByIdentifier(identifier) is not null)
                GetMemberByIdentifier(identifier)?.PurchaseBasket(basket);
            else
                GetClientByIdentifier(identifier)?.PurchaseBasket(basket);
        }

        public void NotificationOn(string identifier)
        {
            if(GetMemberByIdentifier(identifier) is not null)
                GetMemberByIdentifier(identifier)?.NotificationOn();
            else
                {
                    throw new Exception($"{identifier} not logged in");
                }
        }

        public void NotificationOff(string identifier)
        {
            if(GetMemberByIdentifier(identifier) is not null)
                GetMemberByIdentifier(identifier)?.NotificationOff();
            else
                {
                    throw new Exception($"{identifier} not logged in");
                }
        }

        public void SetMemberNotifications(string identifier, bool on)
        {
            if(on){
                NotificationOn(identifier);
            }else{
                NotificationOff(identifier);
            }
        }
        public string GetTokenByUserName(string userName)
        {
            return MemberByToken.Where(pair => pair.Value.UserName == userName).FirstOrDefault().Key;
        }
    }
   
}
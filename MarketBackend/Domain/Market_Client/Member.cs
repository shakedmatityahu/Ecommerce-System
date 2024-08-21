using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using MarketBackend.DAL;
using MarketBackend.DAL.DTO;
using MarketBackend.Domain.Models;

namespace MarketBackend.Domain.Market_Client
{
    public class Member : Client
    {
        public string UserName {get; set;}
        public string Password {get; set;}
        public MailAddress Email {get; set;}
        // public ConcurrentDictionary<int, Role> Roles {get; set;}
        public ConcurrentDictionary<int,ShoppingCartHistory> OrderHistory {get; set;}
        public bool IsSystemAdmin {get; set;}
        public bool IsLoggedIn {get; set;}
        public bool IsNotification {get; set;}

        object _lock = new Object();

        public SynchronizedCollection<Message> alerts;
        public NotificationManager _alertManager = NotificationManager.GetInstance();

        public Member(int id, string userName, MailAddress mailAddress, string password) : base(id)
        {
            UserName = userName;
            Password = password;
            Email = mailAddress;
            // Roles = new(); 
            OrderHistory = new(); 
            IsSystemAdmin = false;
            IsLoggedIn = false;
            IsNotification = true; //TODO maybe change to true
            alerts = new SynchronizedCollection<Message>();
        }

        public Member(MemberDTO other) : base(other.Id)
        {            
            UserName = other.UserName;
            Password = other.Password;
            OrderHistory = new ConcurrentDictionary<int, ShoppingCartHistory>();
            foreach (ShoppingCartHistoryDTO historyDTO in other?.OrderHistory??new())
            {
                ShoppingCartHistory history = new(historyDTO);
                OrderHistory.TryAdd(history._shoppingCartId, history);
            }
            // Roles = new ConcurrentDictionary<int, Role>();
            // foreach (RoleDTO roleDTO in other.Roles)
            // {
            //     Role role = new(roleDTO);
            //     Roles.TryAdd(role.storeId, role);
            // }
            IsSystemAdmin = other.IsSystemAdmin;
            IsLoggedIn = false;
            IsNotification = other.IsNotification;
            alerts = new (other.Alerts);
        }
       
        public override void PurchaseBasket(Basket basket)
        {
            if(!OrderHistory.TryGetValue(basket._cartId, out var cartInHistory)){
                cartInHistory ??= new(){_shoppingCartId = basket._cartId};
                OrderHistory.TryAdd(basket._cartId, cartInHistory);
                BasketRepositoryRAM.GetInstance().Add_cartHistory(cartInHistory, UserName);
            }            
            cartInHistory.AddBasket(basket);            
            base.PurchaseBasket(basket);
        }

        public override void AddToCart(int storeId, int productId, int quantity){
            Cart.addToCart(storeId, productId, quantity);
        }
        
        
        public override void RemoveFromCart(int storeId, int productId, int quantity){
            Cart.removeFromCart(storeId, productId, quantity);
        }

        public List<ShoppingCartHistory> GetHistory()
        {
            return OrderHistory.Values.ToList();
        }

        public void Notify(string msg)
        {
            var message = new Message(msg);

            if (IsNotification && IsLoggedIn)
            {
                _alertManager.SendNotification(msg, UserName);
                message.Seen = true;
            }
            else{
                alerts.Add(message);            
            }

        }

        public void NotificationOn()
        {
            if (!IsNotification)
            {
                IsNotification = true;
                ClientRepositoryRAM.GetInstance().Update(this);
            }
            else throw new Exception("Notification On");
        }

        public void NotificationOff()
        {
            if (IsNotification)
            {
                IsNotification = false;
                ClientRepositoryRAM.GetInstance().Update(this);
            }
            else throw new Exception("Notification Off");
        }

        public List<Message> GetMessages()
        {
            lock (_lock)
            {                
                return alerts.ToList();
            }
        }

    }
    
}
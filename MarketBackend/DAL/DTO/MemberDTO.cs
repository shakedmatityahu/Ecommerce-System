using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mail;
using System.ServiceModel.Channels;
using MarketBackend.Domain.Market_Client;
using MarketBackend.Domain.Models;
using Message = MarketBackend.Domain.Market_Client.Message;

namespace MarketBackend.DAL.DTO
{
    [Table("Members")]
    public class MemberDTO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public List<MessageDTO> Alerts { get; set; } = new ();
        public ShoppingCartDTO ShoppingCart { get; set; }
        public bool IsSystemAdmin { get; set; }
        public string UserName {get; set;}
        public string Password {get; set;}
        // public List<RoleDTO> Roles {get; set;}
        public List<ShoppingCartHistoryDTO> OrderHistory {get; set;}
        public bool IsNotification {get; set;}

        public MemberDTO() { }
        public MemberDTO(int id, string userName, string password, List<MessageDTO> alerts, bool notification, List<ShoppingCartHistoryDTO> orderHistory, ShoppingCartDTO shoppingCart)
        {
            Id = id;
            UserName = userName;
            Password = password;
            Alerts = alerts;
            IsNotification = notification;
            ShoppingCart = shoppingCart;
            OrderHistory = orderHistory;
            IsSystemAdmin = false;
        }
        public MemberDTO(int id, string userName, string password, List<MessageDTO> alerts, bool notification, ShoppingCartDTO shoppingCart)
        {
            Id = id;
            UserName = userName;
            Password = password;
            if (alerts != null) Alerts = alerts; else Alerts = new ();
            IsNotification = notification;
            ShoppingCart = shoppingCart;
            OrderHistory = new ();
            IsSystemAdmin = false;
        }

        public MemberDTO(int id, string userName, string password, List<MessageDTO> alerts, bool notification, ShoppingCartDTO shoppingCart, List<ShoppingCartHistoryDTO> orderHistory)
        {
            Id = id;
            UserName = userName;
            Password = password;
            if (alerts != null) Alerts = alerts; else Alerts = new ();
            IsNotification = notification;
            ShoppingCart = shoppingCart;
            OrderHistory = orderHistory;
            IsSystemAdmin = false;
        }
        public MemberDTO(Member member)
        {
            Id = member.Id;
            UserName = member.UserName;
            Password = member.Password;
            Alerts = new List<MessageDTO>();
            foreach (Message message in member.alerts)
            {
                Alerts.Add(new MessageDTO(message));
            }
            IsNotification = member.IsNotification;
            ShoppingCart = new ShoppingCartDTO(member.Cart);
            OrderHistory = new ();
            foreach (var order in member.OrderHistory.Values.ToList())
                OrderHistory.Add(new ShoppingCartHistoryDTO(order));
            IsSystemAdmin = member.IsSystemAdmin;
        }
    }
}

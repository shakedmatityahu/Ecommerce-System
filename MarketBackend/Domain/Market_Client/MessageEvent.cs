
namespace MarketBackend.Domain.Market_Client
{
    public class MessageEvent : Event
    {
        private string _userName;
        private string _message;

        public MessageEvent(string userName, string message) : base("Report Event")
        {
            _userName = userName;
            _message = message;
        }

        public string UserName { get => _userName; set => _userName = value; }

        public override string GenerateMsg()
        {
            return $"{Name}: From: {_userName}, Msg: {_message}";
        }

    }
}
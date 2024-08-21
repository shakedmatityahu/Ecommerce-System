
namespace MarketBackend.Domain.Market_Client
{
    public class Message
    {
        public string Comment {get; set;}
        public bool Seen {get; set;}

        public Message(string comment)
        {
            this.Comment = comment;
            Seen = false;
        }

        public Message(string comment,bool seen)
        {
            this.Comment = comment;
            Seen = seen;
        }

    }
}
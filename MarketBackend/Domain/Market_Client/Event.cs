
namespace MarketBackend.Domain.Market_Client{
    public abstract class Event
    {
        private string _name;
        public Event(string name){
            _name = name;
        }

        public string Name { get => _name; set => _name = value; }
        public void Update(Member member)
        {
            member.Notify(GenerateMsg());
        }

        public abstract string GenerateMsg();
    }
} 
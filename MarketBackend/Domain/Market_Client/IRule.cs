using MarketBackend.DAL.DTO;
using Microsoft.EntityFrameworkCore;

namespace MarketBackend.Domain.Market_Client{
    public abstract class IRule{
        private int _id;
        private RuleSubject _subject;
        private int _storeId;

        public int Id { get => _id; set => _id = value; }
        public int storeId { get => _storeId; set => _storeId = value; }
        public RuleSubject Subject { get => _subject; set => _subject = value; }

        public IRule(int id, RuleSubject subject, int storeId){
            _id = id;
            _subject = subject;
            _storeId = storeId;
        }

        public IRule(int id, int storeId){
            _id = id;
            _storeId = storeId;
        }

        public IRule(RuleDTO ruleDTO) {
            _subject = new RuleSubject(ruleDTO.Subject);
            _id = ruleDTO.Id;
            _storeId = DBcontext.GetInstance().Stores
                .Include(s => s.Rules)
                .FirstOrDefault(s => s.Rules.Any(rule => rule.Id == ruleDTO.Id))
                .Id;
        }

        public abstract string GetInfo();
        public abstract bool Predicate(Basket basket);
        public abstract void Update();

        public abstract RuleDTO CloneDTO();
    }
}
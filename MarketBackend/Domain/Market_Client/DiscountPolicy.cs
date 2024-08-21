using MarketBackend.DAL.DTO;

namespace MarketBackend.Domain.Market_Client
{
public class DiscountPolicy : IPolicy
    {
        private double _precentage;

        public double Precentage { get => _precentage; set => _precentage = value; }

        public DiscountPolicy(int id, int storeId, DateTime expirationDate, RuleSubject subject, IRule rule, double percentage) : base(id, storeId, expirationDate, subject, rule)
        {
            Precentage = percentage;
        }

        public DiscountPolicy(int id,int storeId, DateTime expirationDate, RuleSubject subject) : base(id,storeId, expirationDate, subject)
        {
        }

        public DiscountPolicy(DiscountPolicyDTO discountPolicyDTO):base(discountPolicyDTO)
        {
            _precentage = discountPolicyDTO.Precentage;
        }

        
        public override void Apply(Basket basket)
        {
            if (!IsExpired() && IsValidForBasket(basket))
            {
                RuleSubject subjectToDiscount = Subject;
                if (subjectToDiscount.IsProduct())
                    ApplyOnProduct(basket, subjectToDiscount.Product);
                else ApplyOnCategory(basket, subjectToDiscount.Category);
            }
        }
        private void ApplyOnProduct(Basket basket, Product product)
        {
            BasketItem basketItem = basket.GetBasketItem(product);
            if(basketItem!=null && IsRegularSellProduct(basketItem))
            {
                basketItem.PriceAfterDiscount -= basketItem.PriceAfterDiscount * Precentage;
            }
        }
        private bool IsRegularSellProduct(BasketItem basketItem)
        {
            return basketItem.Product.SellMethod is RegularSell;
        }
        private void ApplyOnCategory(Basket basket, string category)
        {
            foreach (BasketItem basketItem in basket.BasketItems)
            {
                if (basketItem.Product.HasCategory(category)&&IsRegularSellProduct(basketItem))
                    basketItem.PriceAfterDiscount -= basketItem.Product.Price * Precentage;
            }
        }

        public override string GetInfo()
        {
            return $"Simple Discount: Subject: {Rule.Subject.GetInfo()}, Precentage: {_precentage * 100}";
        }

        public override bool IsValidForBasket(Basket basket)
        {
            return Rule.Predicate(basket);
        }
        public double GetDiscount(Basket basket)
        {
            RuleSubject subjectToDiscount = Rule.Subject;                
            if (!IsValidForBasket(basket))
                return 0;

            if (!subjectToDiscount.IsProduct())
                return GetCategoryDiscount(basket, subjectToDiscount.Category);

            else
                return GetProductDiscount(basket, subjectToDiscount.Product);
        }
        private double GetCategoryDiscount(Basket basket,string category)
        {
            double priceToReduce = 0;
            foreach (BasketItem basketItem in basket.BasketItems)
            {
                if (basketItem.Product.HasCategory(category))
                priceToReduce += CalculateDiscount(basketItem.Quantity, basketItem.Product.Price);
            }
            return priceToReduce;
        }
        private double GetProductDiscount(Basket basket, Product product)
        {
            double priceToReduce = 0;
            BasketItem basketItem = basket.GetBasketItem(product);
            priceToReduce += CalculateDiscount(basketItem.Quantity, basketItem.Product.Price);
            return priceToReduce;
        }
        private double CalculateDiscount(int quantity, double price)
        {
            return quantity * price * Precentage;
        }
        public override DiscountPolicyDTO CloneDTO()
        {
            return new DiscountPolicyDTO(this);
        }
    }
}
using MarketBackend.DAL;
using MarketBackend.DAL.DTO;

namespace MarketBackend.Domain.Market_Client{
    public class BasketItem{
        private Product _product;
        private double _priceAfterDiscount;
        private int _quantity;
        public Product Product { get => _product; set => _product = value; }
        public double PriceAfterDiscount { get => _priceAfterDiscount; set => _priceAfterDiscount = value; }
        public int Quantity { get => _quantity; set => _quantity = value; }
        public BasketItem(Product product, int quantity)
        {
            _product = product;
            _priceAfterDiscount = product.Price;
            _quantity = quantity;
        }

        public BasketItem(Product product, int quantity, double priceAfterDiscount)
        {
            _product = product;
            _priceAfterDiscount = priceAfterDiscount;
            _quantity = quantity;
        }

        // public BasketItem(PurchasedItemDTO purchasedItemDTO)
        // {
        //     _product = ProductRepositoryRAM.GetInstance().GetById(purchasedItemDTO.ProductId);
        //     _priceAfterDiscount = purchasedItemDTO.PriceAfterDiscount;
        //     _quantity = purchasedItemDTO.Quantity;
        // }

        public BasketItem(BasketItemDTO basketItemDTO)
        {
            _product = ProductRepositoryRAM.GetInstance().GetById(basketItemDTO.Product.ProductId);
            _priceAfterDiscount = basketItemDTO.PriceAfterDiscount;
            _quantity = basketItemDTO.Quantity;
        }

        internal BasketItem Clone()
        {
            Product productCoppy = _product.Clone();
            double priceAfterDiscount = _priceAfterDiscount;
            int quantity = _quantity;
            return new BasketItem(productCoppy,quantity,priceAfterDiscount);
        }


    }
}
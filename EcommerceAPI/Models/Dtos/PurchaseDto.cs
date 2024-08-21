using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketBackend.Domain.Payment;
using MarketBackend.Domain.Shipping;
using WebSocketSharp;

namespace EcommerceAPI.Models.Dtos
{
    public class PurchaseDto
    {
        public int StoreId { get; set; }

        public string? ZipCode { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public string? HolderId { get; set; }
        public string? CVV { get; set; }
        public string? CardHolder { get; set; }
        public string? ExpYear { get; set; }
        public string? ExpMonth { get; set; }
        public string? CardNumber { get; set; }

        public string? Currency { get; set; }



        public bool IsValid()
        {
            return 
                !CardNumber.IsNullOrEmpty() 
                && !CardHolder.IsNullOrEmpty() 
                && !HolderId.IsNullOrEmpty() 
                && !ZipCode.IsNullOrEmpty() 
                && !Country.IsNullOrEmpty() 
                && !City.IsNullOrEmpty()             
                && !Address.IsNullOrEmpty() 
                && !CVV.IsNullOrEmpty()
                && !ExpYear.IsNullOrEmpty()
                && !ExpMonth.IsNullOrEmpty()
                && !Currency.IsNullOrEmpty();
        }
        public ShippingDetails ShippingInfo()
        {
            return new(CardHolder, City, Address, Country, ZipCode);
        }
        public PaymentDetails PaymentInfo()
        {
            return new(Currency, CardNumber, ExpYear, ExpMonth, CVV, HolderId, CardHolder);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceAPI.Models.Dtos
{
    public class ProductDto
    {
        public int StoreId { get; set; }
        public int? Id { get; set; }
        public string? ProductName { get; set; }
        public string? SellMethod { get; set; }
        public string? ProductDescription { get; set; }
        public double? Price { get; set; }
        public int Quantity { get; set; }
        public string? Category { get; set; }
        public bool AgeLimit { get; set; }

        public bool IsValidCreate()
        {
            return StoreId != 0 && ProductName is not null && Price is not null;
        }
        public bool IsValidForCart()
        {
            return StoreId != 0 && Id is not null  && Quantity != 0;
        }


    }
}
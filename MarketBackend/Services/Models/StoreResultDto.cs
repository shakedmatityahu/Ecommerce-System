using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketBackend.Domain.Market_Client;

namespace MarketBackend.Services.Models
{
    public class StoreResultDto
    {
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public bool Active { get; set; }
        public List<RuleResultDto> Rules { get; set; }
        public string StorePhoneNum { get; set; }
        public string StoreEmailAdd { get; set; }
        public List<ProductResultDto> Products { get; set; }
        public List<RoleResultDto> Roles { get; set; }
        public double Rating { get; set; }

        public StoreResultDto(Store store)
        {
            StoreId = store.StoreId;
            StoreName = store.Name;
            Active = store.Active;
            StorePhoneNum = store._storePhoneNum;
            StoreEmailAdd = store._storeEmailAdd;
            Rating = store._raiting;
            
            Products = store.Products.Select(p => new ProductResultDto(p)).ToList();
            Roles = store.roles.Values.Select(r => new RoleResultDto(r)).ToList();
            Rules = store._rules.Values.Select(r => new RuleResultDto(r)).ToList();
        }
    }
}

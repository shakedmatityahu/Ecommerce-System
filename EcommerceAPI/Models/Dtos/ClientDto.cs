using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebSocketSharp;

namespace EcommerceAPI.Models.Dtos
{
    public class ClientDto
    {
        public string? Username { get; set; }
        public string? Password { get; set; }

        public bool IsValid()
        {
            return Username is not null && Password is not null;
        }
    }

    public class ExtendedClientDto : ClientDto
    {
        public string? Email { get; set; }
        public int Age { get; set; }

        public bool IsValid()
        {            
            return base.IsValid() && !Email.IsNullOrEmpty() && Age != 0;
        }
    }
}
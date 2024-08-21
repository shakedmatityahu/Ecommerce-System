using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketBackend.Domain.Market_Client;

namespace MarketBackend.Services.Models
{
    public class MessageResultDto
    {
       
        public string Comment {get; set;}
        public bool Seen {get; set;}

        public MessageResultDto(Message message)
        {
            this.Comment = message.Comment;
            Seen = message.Seen;
        }        
    }
}
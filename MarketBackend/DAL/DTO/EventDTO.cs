using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MarketBackend.Domain.Market_Client;

namespace MarketBackend.DAL.DTO
{
    [Table("Events")]
    public class EventDTO
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int StoreId { get; set; }
        public MemberDTO Listener { get; set; }
        public EventDTO() { }
        public EventDTO(string name, int storeId, MemberDTO listener)
        {
            Name = name;
            StoreId = storeId;
            Listener = listener;
        }
    }
}


using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketBackend.Domain.Shipping
{
    public interface IShippingSystemFacade
    {

        int CancelShippment(int orderID);
        int OrderShippment(ShippingDetails details);       

        bool Connect();     

        void Disconnect(); 
    }
}

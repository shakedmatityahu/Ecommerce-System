using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketBackend.Domain.Shipping
{
    public class ShippingSystemProxy : IShippingSystemFacade
    {
        private RealShippingSystem? _realShippingSystem;
        public static bool connected;
        private static int fakeTransactionId = 10000;

        public ShippingSystemProxy(RealShippingSystem? realShippingSystem = null)
        {
            _realShippingSystem = realShippingSystem;
            connected = false;
        }

        public ShippingSystemProxy()
        {
            _realShippingSystem = null;
            connected = false;
        }

        public bool Connect()
        {
            if (_realShippingSystem == null)
            {
                connected = true;
                return true;
            }
            else if (_realShippingSystem.Connect())
            {
                connected = true;
                return true;
            }
            else
            {
                connected = false;
                return false;
            }
        }

        public int CancelShippment(int orderID)
        {
            if (connected)
            {
                if (_realShippingSystem == null)
                {
                    return 1;
                }
                else
                {
                    return _realShippingSystem.CancelShippment(orderID);
                }
            }
            return -1;
        }

        public int OrderShippment(ShippingDetails details)
        {
            if (connected)
            {
                if (_realShippingSystem == null)
                {
                    return fakeTransactionId++;
                }
                else
                {
                    return _realShippingSystem.OrderShippment(details);
                }
            }
            return -1;
        }

        public void Disconnect() //for testing
        {
            connected = false;
        }
    }
}
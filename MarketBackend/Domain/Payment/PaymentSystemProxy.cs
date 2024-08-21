using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketBackend.Domain.Payment
{
    public class PaymentSystemProxy : IPaymentSystemFacade
    {
        private readonly RealPaymentSystem? _realPaymentSystem;
        public static bool connected;
        private static int fakeTransactionId = 10000;

        public PaymentSystemProxy(RealPaymentSystem realPaymentSystem)
        {
            _realPaymentSystem = realPaymentSystem ?? throw new ArgumentNullException(nameof(realPaymentSystem));
            connected = false;
        }

        public PaymentSystemProxy()
        {
            _realPaymentSystem = null;
            connected = false;
        }


        public bool Connect()
        {
            if (_realPaymentSystem == null)
            {
                connected = true;
                return true;
            }
            else if (_realPaymentSystem.Connect())
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

        public int Pay(PaymentDetails cardDetails, double totalAmount)
        {
            if (totalAmount <= 0)
                return -1;
            if (connected)
            {
                if (_realPaymentSystem == null)
                    return fakeTransactionId++;
                else
                    return _realPaymentSystem.Pay(cardDetails, totalAmount);
            }
            else
                return -1;
        }

        public int CancelPayment(int paymentID)
        {
            if (connected)
            {
                if (_realPaymentSystem == null)
                    return 1;
                else
                    return _realPaymentSystem.CancelPayment(paymentID);
            }
            else
                return -1;
        }

        public void Disconnect() //for testing
        {
            connected = false;
        }
    }
}
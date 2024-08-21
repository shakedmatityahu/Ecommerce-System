using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketBackend.Domain.Payment
{
    public interface IPaymentSystemFacade
    {
        int Pay(PaymentDetails cardDetails, double totalAmount);
        int CancelPayment(int paymentID);

        bool Connect();

        void Disconnect();
    }
}
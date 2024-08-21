using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Transactions;

namespace MarketBackend.Domain.Payment
{
    public class RealPaymentSystem : IPaymentSystemFacade
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly string _url;

        static RealPaymentSystem()
        {
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(10)
            };
        }

        public RealPaymentSystem(string URL)
        {
            _url = URL;
        }

        public virtual int Pay(PaymentDetails cardDetails, double totalAmount)
        {
            if (!Connect() || totalAmount <= 0)
                return -1;

            var parameters = new Dictionary<string, string>
            {
                { "action_type", "pay" },
                { "amount", totalAmount.ToString() },
                { "currency", cardDetails.Currency },
                { "card_number", cardDetails.CardNumber },
                { "month", cardDetails.ExprMonth },
                { "year", cardDetails.ExprYear },
                { "holder", cardDetails.HolderName },
                { "cvv", cardDetails.Cvv },
                { "id", cardDetails.HolderID }
            };

            var payContent = new FormUrlEncodedContent(parameters); //wrap the request as post content
            try
            {
                var responseTask = _httpClient.PostAsync(_url, payContent);
                var completedTask = Task.WhenAny(responseTask, Task.Delay(TimeSpan.FromSeconds(10))).Result;

                if (completedTask == responseTask)
                {
                    var response = responseTask.Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = response.Content.ReadAsStringAsync().Result;
                        if (!responseContent.Equals("-1"))
                            return int.Parse(responseContent);
                    }
                }
            }
            catch (Exception)
            {
                // Handle exceptions if needed
            }

            return -1;
        }

        public int CancelPayment(int transactionId)
        {
            if (!Connect() || transactionId < 10000 || transactionId > 1000000)
            {
                return -1;
            }

            var parameters = new Dictionary<string, string>
            {
                { "action_type", "cancel_pay" },
                { "transaction_id", transactionId.ToString() }
            };

            var cancelPayContent = new FormUrlEncodedContent(parameters); //wrap the request as post content
            try
            {
                var responseTask = _httpClient.PostAsync(_url, cancelPayContent);
                var completedTask = Task.WhenAny(responseTask, Task.Delay(TimeSpan.FromSeconds(10))).Result;

                if (completedTask == responseTask)
                {
                    var response = responseTask.Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = response.Content.ReadAsStringAsync().Result;
                        if (responseContent.Equals("1")) //success
                        {
                            return 1;
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Handle exceptions if needed
            }

            return -1;
        }

        public bool Connect()
        {
            var content = new Dictionary<string, string>
            {
                { "action_type", "handshake" }
            };

            var handshakeContent = new FormUrlEncodedContent(content); //wrap the request as post content
            try
            {
                var responseTask = _httpClient.PostAsync(_url, handshakeContent);
                var completedTask = Task.WhenAny(responseTask, Task.Delay(TimeSpan.FromSeconds(10))).Result;

                if (completedTask == responseTask)
                {
                    var response = responseTask.Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = response.Content.ReadAsStringAsync().Result;
                        if (responseContent.Equals("OK"))
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Handle exceptions if needed
            }

            return false;
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }
    }
}

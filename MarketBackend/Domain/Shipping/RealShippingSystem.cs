using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace MarketBackend.Domain.Shipping
{
    public class RealShippingSystem : IShippingSystemFacade
    {
        private readonly HttpClient _httpClient;
        private readonly string _url;

        public RealShippingSystem(string URL)
        {
            _url = URL;
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(10)
            };
        }

        public virtual int OrderShippment(ShippingDetails details)
        {
            if (!Connect() || details == null)
                return -1;

            var parameters = new Dictionary<string, string>
            {
                { "action_type", "supply" },
                { "name", details.Name },
                { "address", details.Address },
                { "city", details.City },
                { "country", details.Country },
                { "zip", details.Zipcode }
            };

            var supplyContent = new FormUrlEncodedContent(parameters);
            try
            {
                var responseTask = _httpClient.PostAsync(_url, supplyContent);
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

        public int CancelShippment(int orderID)
        {
            if (!Connect() || orderID < 10000 || orderID > 1000000)
                return -1;

            var parameters = new Dictionary<string, string>
            {
                { "action_type", "cancel_supply" },
                { "transaction_id", orderID.ToString() }
            };

            var cancelSupplyContent = new FormUrlEncodedContent(parameters);
            try
            {
                var responseTask = _httpClient.PostAsync(_url, cancelSupplyContent);
                var completedTask = Task.WhenAny(responseTask, Task.Delay(TimeSpan.FromSeconds(10))).Result;

                if (completedTask == responseTask)
                {
                    var response = responseTask.Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = response.Content.ReadAsStringAsync().Result;
                        if (responseContent.Equals("1"))
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

        public bool Connect()
        {
            var content = new Dictionary<string, string>
            {
                { "action_type", "handshake" }
            };

            var handshakeContent = new FormUrlEncodedContent(content);
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
                            return true;
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

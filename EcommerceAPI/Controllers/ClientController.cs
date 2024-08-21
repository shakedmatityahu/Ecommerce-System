using Microsoft.AspNetCore.Mvc;
using WebSocketSharp.Server;
using Microsoft.AspNetCore.Session;
using System.Xml.Schema;
using System.Diagnostics.Metrics;
using System.Collections.Generic;
using MarketBackend.Services;
using System.ComponentModel.DataAnnotations;
using EcommerceAPI.Models.Dtos;
using MarketBackend.Services.Interfaces;
using MarketBackend.Domain.Market_Client;
using MarketBackend.Services.Models;
using MarketBackend.Domain.Shipping;
using MarketBackend.Domain.Payment;
using System.Net.Sockets;
using System.Net;


namespace EcommerceAPI.Controllers
{

    [ApiController]
    [Route("api/Client")]
    public class ClientController : ControllerBase
    {
        private readonly WebSocketServer AlertServer;
        private readonly WebSocketServer LogServer;
        private IClientService _clientService;

        private static Dictionary<string, IList<string>> _clientPendingAlerts = new();
        private static Dictionary<string, string> _alertPathByclientIdentifier = new();
        public ClientController(IClientService clientService, WebSocketServer logs, WebSocketServer alerts)
        {
            _clientService = clientService;
            AlertServer =alerts;
            this.LogServer = logs;
            NotificationManager.GetInstance(AlertServer);
        }
        private class NotificationsService : WebSocketBehavior
        {

        }
        public class logsService : WebSocketBehavior
        {

        }
        [HttpPost]
        [Route("Guest/Login")]
        public async Task<ActionResult<ServerResponse<string>>> Login([FromBody] ClientDto client)
        {
            string relativePath = $"/{client.Username}-alerts";
            try
            {
                if (AlertServer.WebSocketServices[relativePath] == null)
                {
                    AlertServer.AddWebSocketService<NotificationsService>(relativePath);
                }
            }
            catch (Exception ex)
            {
                var loginResponse = new ServerResponse<string>
                {
                    ErrorMessage = ex.Message,
                };
                return BadRequest(loginResponse);
            }

            var response = await Task.Run(() => _clientService.LoginClient(client.Username, client.Password));
            if (response.ErrorOccured)
            {
                AlertServer.RemoveWebSocketService(relativePath);
                var loginResponse = new ServerResponse<string>
                {
                    ErrorMessage = response.ErrorMessage,
                };
                return BadRequest(loginResponse);
            }
            else
            {
                _alertPathByclientIdentifier.Add(response.Value, relativePath);
                var createShopResponse = new ServerResponse<string>
                {
                    Value = response.Value,
                };
                return Ok(createShopResponse);
            }
        }


        private void AddPendingAlert(string username, IList<string> messages)
        {
            _clientPendingAlerts[username] = messages;
        }


        [HttpPost]
        [Route("Member/Logout")]
        public async Task<ActionResult<ServerResponse<string>>> Logout([Required][FromQuery]string identifier)
        {
            Response response = await Task.Run(() => _clientService.LogoutClient(identifier));
            ServerResponse<string> logoutResponse;
            if (response.ErrorOccured)
            {
                logoutResponse = new ServerResponse<string>
                {
                    ErrorMessage = response.ErrorMessage,
                };
                return BadRequest(logoutResponse);
            }
            if (_alertPathByclientIdentifier.ContainsKey(identifier))
            {
                AlertServer.RemoveWebSocketService(_alertPathByclientIdentifier[identifier]);
                _alertPathByclientIdentifier.Remove(identifier);
            }
            logoutResponse = new ServerResponse<string>
            {
                Value = "logout success",
            };
            return Ok(logoutResponse);
        }

        [HttpPost]
        [Route("Guest")]
        public async Task<ActionResult<ServerResponse<string>>> EnterAsGuest()
        {
            string session = HttpContext.Session.Id;
            Response response = await Task.Run(() => _clientService.EnterAsGuest(session));
            if (response.ErrorOccured)
            {
                var enterAsGuestResponse = new ServerResponse<string>
                {
                    ErrorMessage = response.ErrorMessage,
                };
                return BadRequest(enterAsGuestResponse);
            }
            else
            {
                var enterAsGuestResponse = new ServerResponse<string>
                {
                    Value = session,
                };
                return Ok(enterAsGuestResponse);
            }
        }

        [HttpPost]
        [Route("Guest/Register")]
        public async Task<ActionResult<ServerResponse<string>>> Register([FromBody] ExtendedClientDto client)
        {
            Response response = await Task.Run(() => _clientService.Register(client.Username, client.Password, client.Email, client.Age));
            if (response.ErrorOccured)
            {
                var registerResponse = new ServerResponse<string>
                {
                    ErrorMessage = response.ErrorMessage,
                };
                return BadRequest(registerResponse);
            }
            else
            {
                var registerResponse = new ServerResponse<string>
                {
                    Value = client.Username,
                };
                return Ok(registerResponse);
            }

        }                                            

        [HttpPut] //client controller
        [Route("Cart")]
        public async Task<ActionResult<ServerResponse<string>>> UpdateCart([Required][FromQuery]string identifier, [FromBody] ProductDto product)
        {
            if(!product.IsValidForCart()) return BadRequest("product must contain id, store id and product name");

            Response response = product.Quantity > 0 ? 
                await Task.Run(() => _clientService.AddToCart(identifier, product.StoreId, (int)product.Id, product.Quantity)) : 
                await Task.Run(() => _clientService.RemoveFromCart(identifier, (int)product.Id, product.StoreId, Math.Abs(product.Quantity)));
            if (response.ErrorOccured)
            {
                var addToCartResponse = new ServerResponse<string>
                {
                    ErrorMessage = response.ErrorMessage,
                };
                return BadRequest(addToCartResponse);
            }
            else
            {
                var addToCartResponse = new ServerResponse<string>
                {
                    Value = "Cart update success",
                };
                return Ok(addToCartResponse);
            }
        }

        [HttpGet]
        [Route("Cart")]
        public ActionResult<Response<ShoppingCartResultDto>> GetShoppingCartInfo([Required][FromQuery]string identifier)
        {
            Response<ShoppingCartResultDto> response = _clientService.ViewCart(identifier);
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.BadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<ShoppingCartResultDto>.OkResponse(response.Value));
            }
        }                

        [HttpGet]
        [Route("Member/PurchaseHistory")]
        public ActionResult<Response<List<ShoppingCartResultDto>>> GetMemberPurchaseHistory([Required][FromQuery]string userName)
        {
            Response<List<ShoppingCartResultDto>> response = _clientService.GetPurchaseHistoryByClient(userName);
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.BadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<List<ShoppingCartResultDto>>.OkResponse(response.Value));
            }
        }   

        [HttpPost]
        [Route("Client/CreateStore")]
        public async Task<ObjectResult> CreateStore([Required][FromQuery]string identifier, [Required][FromBody] StoreDto storeInfo)
        {
            Response<int> response = await Task.Run(() => _clientService.CreateStore(identifier, storeInfo.Name, storeInfo.Email, storeInfo.PhoneNum));
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.BadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<int>.OkResponse(response.Value));
            }
        }             

        [HttpGet]
        [Route("Client/Stores")]
        public async Task<ObjectResult> GetMemberStores([Required][FromQuery]string identifier)
        {
            Response<List<StoreResultDto>> response = await Task.Run(() => _clientService.GetMemberStores(identifier));
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.BadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<List<StoreResultDto>>.OkResponse(response.Value));
            }
        }   

        [HttpGet]
        [Route("Client/Stores/{storeId}")]
        public async Task<ObjectResult> GetMemberStore([Required][FromQuery]string identifier, [FromRoute] int storeId)
        {
            Response<StoreResultDto> response = await Task.Run(() => _clientService.GetMemberStore(identifier, storeId));
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.BadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<StoreResultDto>.OkResponse(response.Value));
            }
        }

        [HttpGet]
        [Route("Client/Notifications")]
        public async Task<ObjectResult> GetMemberNotifications([Required][FromQuery]string identifier)
        {
            Response<List<MessageResultDto>> response = await Task.Run(() => _clientService.GetMemberNotifications(identifier));
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.BadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<List<MessageResultDto>>.OkResponse(response.Value));
            }
        }

        [HttpPost]
        [Route("Client/Notifications")]
        public async Task<ObjectResult> SetMemberNotifications([Required][FromQuery]string identifier, [FromQuery]bool on)
        {
            Response response = await Task.Run(() => _clientService.SetMemberNotifications(identifier, on));
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.BadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<string>.OkResponse("succses"));
            }
        }

        [HttpPost]
        [Route("Guest/exit")]
        public async Task<ActionResult<ServerResponse<string>>> ExitGuest([Required][FromQuery]string identifier)
        {
            Response response = await Task.Run(() => _clientService.ExitGuest(identifier));
            if (response.ErrorOccured)
            {
                var exitGuestResponse = new ServerResponse<string>
                {
                    ErrorMessage = response.ErrorMessage,
                };
                return BadRequest(exitGuestResponse);
            }
            else
            {
                var exitGuestResponse = new ServerResponse<string>
                {
                    Value = "exit success",
                };
                return Ok(exitGuestResponse);
            }
        }

        [HttpPost]
        [Route("Client/ResManager")]
        public async Task<ObjectResult> ResToStoreManageReq([Required][FromQuery]string identifier)
        {
            Response<bool> response = await Task.Run(() => _clientService.ResToStoreManageReq(identifier));
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.BadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<bool>.OkResponse(response.Value));
            }
        }

        
        [HttpPost]
        [Route("Client/ResOwner")]
        public async Task<ObjectResult> ResToStoreOwnershipReq([Required][FromQuery]string identifier)
        {
            Response<bool> response = await Task.Run(() => _clientService.ResToStoreOwnershipReq(identifier));
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.BadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<bool>.OkResponse(response.Value));
            }
        }
        static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}

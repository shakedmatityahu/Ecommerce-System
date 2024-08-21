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
using MarketBackend.Domain.Payment;
using MarketBackend.Services.Models;
using MarketBackend.Domain.Shipping;

namespace EcommerceAPI.Controllers
{
    public class ServerResponse<T>
    {
        public T Value { get; set; }
        public string ErrorMessage { get; set; }
        public static ServerResponse<T> OkResponse(T val)
        {
            var response = new ServerResponse<T>
            {
                Value = val,
            };
            return response;
        }
        public static ServerResponse<T> BadResponse(string msg)
        {
            var response = new ServerResponse<T>
            {
                ErrorMessage = msg,
            };
            return response;
        }
        
    }

    [ApiController]
    [Route("api/Market")]
    public class MarketController : ControllerBase
    {
        private readonly WebSocketServer AlertServer;
        private readonly WebSocketServer LogServer;
        private IMarketService _marketService;

        private static IDictionary<string, IList<string>> buyerUnsentMessages = new Dictionary<string, IList<string>>();
        private static IDictionary<string, string> buyerIdToRelativeNotificationPath = new Dictionary<string, string>();
        public MarketController(IMarketService marketService, WebSocketServer alerts)
        {
            _marketService = marketService;
            AlertServer = alerts;
            // LogServer = logs;
            NotificationManager.GetInstance(AlertServer);
        }
        private class NotificationsService : WebSocketBehavior
        {

        }
        public class logsService : WebSocketBehavior
        {

        }                  

        // [HttpPost]
        // [Route("Staff")]
        // public async Task<ObjectResult> Appoint([Required][FromQuery]string identifier, [FromBody] StaffMemberDto staffMember)
        // {
        //     var role = GetRoleByName(staffMember.Role);
        //     Response<int> response = await Task.Run(() => _marketService.AddStaffMember(staffMember.StoreId, identifier, new Role(new(role), new(), staffMember.StoreId, staffMember.Id), staffMember.Id));
            
        //     if (response.ErrorOccured)
        //     {
        //         var appointResponse = new ServerResponse<string>
        //         {
        //             ErrorMessage = response.ErrorMessage,
        //         };
        //         return BadRequest(appointResponse);
        //     }
        //     else
        //     {
        //         var appointResponse = new ServerResponse<string>
        //         {
        //             Value = "Appoint success",
        //         };
        //         return Ok(appointResponse);
        //     }
        //     return Ok(response);

        // }

        private RoleName GetRoleByName(string description)
        {
            foreach (RoleName roleName in Enum.GetValues(typeof(RoleName)))
            {
                if (roleName.GetDescription().Equals(description, StringComparison.OrdinalIgnoreCase))
                {
                    return roleName;
                }
            }
            return default;
        }

        [HttpPost]
        [Route("Store/{storeId}/Staff")]
        public async Task<ObjectResult> AddStaff([Required][FromQuery]string identifier, [FromRoute] int storeId, [FromBody] StaffMemberDto staffMember)
        {
            Response response = await Task.Run(() => _marketService.AddStaffMember(storeId, identifier, staffMember.RoleName, staffMember.MemberUserName));
            if (response.ErrorOccured)
            {
                var removeAppointResponse = new ServerResponse<string>
                {
                    ErrorMessage = response.ErrorMessage,
                };
                return BadRequest(removeAppointResponse);
            }
            else
            {
                var removeAppointResponse = new ServerResponse<string>
                {
                    Value = "add Appoint success",
                };
                return Ok(removeAppointResponse);
            }
        }

        [HttpDelete]
        [Route("Store/{storeId}/Staff")]
        public async Task<ObjectResult> RemoveStaff([Required][FromQuery]string identifier, [FromRoute] int storeId, [FromBody] StaffMemberDto staffMember)
        {
            Response response = await Task.Run(() => _marketService.RemoveStaffMember(storeId, identifier, staffMember.MemberUserName));
            if (response.ErrorOccured)
            {
                var removeAppointResponse = new ServerResponse<string>
                {
                    ErrorMessage = response.ErrorMessage,
                };
                return BadRequest(removeAppointResponse);
            }
            else
            {
                var removeAppointResponse = new ServerResponse<string>
                {
                    Value = "add Appoint success",
                };
                return Ok(removeAppointResponse);
            }
        }

        [HttpPost]
        [Route("Store/{storeId}/Permisions")]
        public async Task<ObjectResult> AddPermission([Required][FromQuery]string identifier, [FromRoute] int storeId, [FromBody] StaffMemberDto staffMember)
        {            
            var permissionTasks = new List<Task<Response>>();
            foreach(var permission in staffMember.Permission){
                permissionTasks.Add(Task.Run(() => _marketService.AddPermission(identifier, storeId, staffMember.MemberUserName, permission)));
            }
            await Task.WhenAll(permissionTasks.ToArray());
            var responses = permissionTasks.Select(task => task.Result);
            
            foreach(var response in responses){
                    if(response.ErrorOccured)
                        return BadRequest(ServerResponse<string>.BadResponse(response.ErrorMessage));
            }
           
            var addPermissionResponse = new ServerResponse<string>
            {
                Value = "add permission success",
            };
            return Ok(addPermissionResponse);
        }
        
        [HttpDelete]
        [Route("Store/{storeId}/Permisions")]
        public async Task<ObjectResult> RemovePermission([Required][FromQuery]string identifier, [FromRoute] int storeId, [FromBody] StaffMemberDto staffMember)
        {

            var permissionTasks = new List<Task<Response>>();
            foreach(var permission in staffMember.Permission){
                permissionTasks.Add(Task.Run(() => _marketService.RemovePermission(identifier, storeId, staffMember.MemberUserName, permission)));
            }
            await Task.WhenAll(permissionTasks.ToArray());
            var responses = permissionTasks.Select(task => task.Result);
            
            foreach(var response in responses){
                    if(response.ErrorOccured)
                        return BadRequest(ServerResponse<string>.BadResponse(response.ErrorMessage));
            }
           
            var removePermissionResponse = new ServerResponse<string>
            {
                Value = "remove permission success",
            };
            return Ok(removePermissionResponse);
        }

        [HttpPost]
        [Route("Store/{storeId}/Close")]
        public async Task<ObjectResult> CloseStore([Required][FromQuery]string identifier, [FromRoute] int storeId)
        {
            Response response = await Task.Run(() => _marketService.CloseStore(identifier, storeId));
            if (response.ErrorOccured)
            {
                var closeShopResponse = new ServerResponse<string>
                {
                    ErrorMessage = response.ErrorMessage,
                };
                return BadRequest(closeShopResponse);
            }
            else
            {
                var closeShopResponse = new ServerResponse<string>
                {
                    Value = "close store success",
                };
                return Ok(closeShopResponse);
            }
        }        

        [HttpPost]
        [Route("Store/{storeId}/Open")]
        public async Task<ObjectResult> OpenStore([Required][FromQuery]string identifier, [FromRoute] int storeId)
        {
            Response response = await Task.Run(() => _marketService.OpenStore(identifier, storeId));
            if (response.ErrorOccured)
            {
                var openShopResponse = new ServerResponse<string>
                {
                    ErrorMessage = response.ErrorMessage,
                };
                return BadRequest(openShopResponse);
            }
            else
            {
                var openShopResponse = new ServerResponse<string>
                {
                    Value = "open-shop-success",
                };
                return Ok(openShopResponse);
            }
        }

        [HttpPost]
        [Route("Purchase")]
        public async Task<ObjectResult> PurchaseCart([Required][FromQuery]string identifier, [FromBody] PurchaseDto purchaseInfo)
        {
            if(!purchaseInfo.IsValid()) return BadRequest("all fileds are required");
            var shippingDetails = purchaseInfo.ShippingInfo();
            var paymentDetails = purchaseInfo.PaymentInfo();
            Response response = await Task.Run(() => _marketService.PurchaseCart(identifier, paymentDetails, shippingDetails));
            if (response.ErrorOccured)
            {
                var purchaseBasketResponse = new ServerResponse<string>
                {
                    ErrorMessage = response.ErrorMessage,
                };
                return BadRequest(purchaseBasketResponse);
            }
            else
            {
                var purchaseBasketResponse = new ServerResponse<string>
                {
                    Value = "purchase success",
                };
                return Ok(purchaseBasketResponse);
            }
        }

        [HttpPost]
        [Route("Store/{storeId}/Products/Remove")]
        public async Task<ObjectResult> RemoveProduct([Required][FromQuery]string identifier, [FromRoute] int storeId, [FromBody] ProductDto product)
        {
            if(product.Id is null) return BadRequest("product must contain id");
            Response response = await Task.Run(() => _marketService.RemoveProduct(storeId, identifier, (int)product.Id));
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.BadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<string>.OkResponse("remove product success"));
            }
        }

        [HttpPost]
        [Route("Store/{storeId}/Products/Add")]
        public async Task<ActionResult<int>> AddProduct([Required][FromQuery]string identifier, [FromRoute] int storeId, [FromBody] ProductDto product)
        {
            if(!product.IsValidCreate()) return BadRequest("product must contain store id and product name");
            Response<int> response = await Task.Run(() => _marketService.AddProduct(storeId, identifier, product.ProductName, product.SellMethod, product.ProductDescription, (double)product.Price, product.Category, product.Quantity, product.AgeLimit));
            if (response.ErrorOccured)
            {
                var addProductResponse = new ServerResponse<string>
                {
                    ErrorMessage = response.ErrorMessage,
                };
                return BadRequest(addProductResponse);
            }
            else
            {
                var addProductResponse = new ServerResponse<int>
                {
                    Value = response.Value,
                };
                return Ok(addProductResponse);
            }
        }              
        
        [HttpPut]
        [Route("Store/{storeId}/Products/{productId}")]
        public async Task<ObjectResult> UpdateProduct([Required][FromQuery]string identifier, [FromRoute] int storeId, [FromRoute] int productId, [FromBody] ProductDto product)
        {
            List<Task<Response>> tasks =
            [
                Task.Run(() => _marketService.UpdateProductPrice(storeId, identifier, productId, (double)product.Price)),
                Task.Run(() => _marketService.UpdateProductQuantity(storeId, identifier, productId, product.Quantity)),
            ];
            var responses = await Task.WhenAll(tasks.ToArray());

            if (responses.Any(response => response.ErrorOccured))
            {
                foreach(var response in responses){
                    if(response.ErrorOccured)
                        return BadRequest(ServerResponse<string>.BadResponse(response.ErrorMessage));
                }
            }
            return Ok(ServerResponse<string>.OkResponse("update product success"));
        }                        

        [HttpGet]
        [Route("Search/KeyWords")]
        public ActionResult<Response<List<ProductResultDto>>> SearchByKeywords([Required][FromQuery]string identifier, [FromQuery] List<string> keyword)
        {
            var products = new List<ProductResultDto>();
            foreach(var word in keyword){
                Response<List<ProductResultDto>> response = _marketService.SearchByKeywords(word);
                if (response.ErrorOccured)
                {
                    return BadRequest(ServerResponse<string>.BadResponse(response.ErrorMessage));
                }else{
                    products.AddRange(response.Value);
                }
            }

            return Ok(ServerResponse<List<ProductResultDto>>.OkResponse(products));
        }

        [HttpGet]
        [Route("Search/Name")]
        public ActionResult<Response<List<ProductResultDto>>> SearchByNames([Required][FromQuery]string identifier, [FromQuery] List<string> name)
        {
            var products = new List<ProductResultDto>();
            foreach(var word in name){
                Response<List<ProductResultDto>> response = _marketService.SearchByName(word);
                if (response.ErrorOccured)
                {
                    return BadRequest(ServerResponse<string>.BadResponse(response.ErrorMessage));
                }else{
                    products.AddRange(response.Value);
                }
            }

            return Ok(ServerResponse<List<ProductResultDto>>.OkResponse(products));
        }

        [HttpGet]
        [Route("Search/Category")]
        public ActionResult<Response<List<ProductResultDto>>> SearchByCategory([Required][FromQuery]string identifier, [FromQuery] List<string> category)
        {
            var products = new List<ProductResultDto>();
            foreach(var word in category){
                Response<List<ProductResultDto>> response = _marketService.SearchByCategory(word);
                if (response.ErrorOccured)
                {
                    return BadRequest(ServerResponse<string>.BadResponse(response.ErrorMessage));
                }else{
                    products.AddRange(response.Value);
                }
            }

            return Ok(ServerResponse<List<ProductResultDto>>.OkResponse(products));
        }

        [HttpGet]
        [Route("Store/Name")]
        public ActionResult<Response<string>> GetStoreById([Required][FromQuery]string identifier, [FromQuery] int storeId)
        {            
            Response<string> response = _marketService.GetStoreById(storeId);
            if (response.ErrorOccured)
            {
                var addProductResponse = new ServerResponse<string>
                {
                    ErrorMessage = response.ErrorMessage,
                };
                return BadRequest(addProductResponse);
            }
            else
            {
                var addProductResponse = new ServerResponse<string>
                {
                    Value = response.Value,
                };
                return Ok(addProductResponse);
            }
        }


        [HttpPost]
        [Route("Store/{storeId}/PurchuseHistory")]
        public ActionResult<Response<List<PurchaseResultDto>>> ShowShopPurchaseHistory([Required][FromQuery]string identifier, [FromRoute] int storeId)
        {
            Response<List<PurchaseResultDto>> response = _marketService.GetPurchaseHistoryByStore(storeId, identifier);
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.BadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<List<PurchaseResultDto>>.OkResponse(response.Value));
            }
        }   

        [HttpGet]
        [Route("Store/{storeId}/Policies/Discount")]
        public ActionResult<Response<List<DiscountPolicyResultDto>>> GetStoreDiscountPolicies([Required][FromQuery]string identifier, [FromRoute] int storeId)
        {
            Response<List<DiscountPolicyResultDto>> response = _marketService.GetStoreDiscountPolicies(storeId, identifier);
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.BadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<List<DiscountPolicyResultDto>>.OkResponse(response.Value));
            }
        }      
        
        [HttpGet]
        [Route("Store/{storeId}/Policies/Purchace")]
        public ActionResult<Response<List<PolicyResultDto>>> GetStorePurchacePolicy([Required][FromQuery]string identifier, [FromRoute] int storeId)
        {
            Response<List<PolicyResultDto>> response = _marketService.GetStorePurchacePolicies(storeId, identifier);
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.BadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<List<PolicyResultDto>>.OkResponse(response.Value));
            }
        }      


        [HttpPost]
        [Route("Store/{storeId}/Policies/Discount")]
        public ActionResult<Response<int>> CreateStoreDiscountPolicy([Required][FromQuery]string identifier, [FromRoute] int storeId, [FromBody] DiscountPolicyDto policy)
        {
            Response<int> response = _marketService.AddDiscountPolicy(identifier, storeId, policy.ExpirationDate, policy.Subject, policy.RuleId, policy.Precantage);
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.BadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<int>.OkResponse(response.Value));
            }
        }   
        
        [HttpPost]
        [Route("Store/{storeId}/Policies/Purchace")]
        public ActionResult<Response> CreateStorePolicy([Required][FromQuery]string identifier, [FromRoute] int storeId, [FromBody] PolicyDto policy)
        {
            Response<int> response = _marketService.AddPurchasePolicy(identifier, storeId, policy.ExpirationDate, policy.Subject, policy.RuleId);
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.BadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<int>.OkResponse(response.Value));
            }
        }         

        [HttpPost]
        [Route("Store/{storeId}/Policies/Composite")]
        public ActionResult<Response<int>> CreateStoreCompositePolicy([Required][FromQuery]string identifier, [FromRoute] int storeId, [FromBody] CompositePolicyDto policy)
        {
            Response<int> response = _marketService.AddCompositePolicy(identifier, storeId, policy.ExpirationDate, policy.Subject, policy.Operator, policy.Policies);
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.BadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<int>.OkResponse(response.Value));
            }
        } 
        
        [HttpDelete]
        [Route("Store/{storeId}/Policies/Purchace")]
        public ActionResult<Response> RemoveStorePolicy([Required][FromQuery]string identifier, [FromRoute] int storeId, [FromQuery] int policyId)
        {
            Response response = _marketService.RemovePolicy(identifier, storeId, policyId, "PurchasePolicy");
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.BadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<string>.OkResponse("remove policy success"));
            }
        }         

        [HttpDelete]
        [Route("Store/{storeId}/Policies/Discount")]
        public ActionResult<Response> RemoveStoreDiscountPolicy([Required][FromQuery]string identifier, [FromRoute] int storeId, [FromQuery] int policyId)
        {
            Response response = _marketService.RemovePolicy(identifier, storeId, policyId, "DiscountPolicy");
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.BadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<string>.OkResponse("remove policy success"));
            }
        }
       

        [HttpGet]
        [Route("Store/{storeId}/GetRules")]
        public ActionResult<Response<List<RuleResultDto>>> GetStoreRules([Required][FromQuery]string identifier, [FromRoute] int storeId)
        {
            Response<List<RuleResultDto>> response = _marketService.GetStoreRules(storeId, identifier);
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.BadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<List<RuleResultDto>>.OkResponse(response.Value));
            }
        }        

        [HttpPost]
        [Route("Store/{storeId}/AddRule")]
        public ActionResult<Response<int>> CreateStoreRule([Required][FromQuery]string identifier, [FromRoute] int storeId, [FromBody] RuleDto rule)
        {
            if(!rule.IsValid()) return BadRequest("rule must have a subject");
            Response<int> response = _marketService.AddSimpleRule(identifier, storeId, rule.Subject);
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.BadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<int>.OkResponse(response.Value));
            }
        }   
        
        [HttpPost]
        [Route("Store/{storeId}/AddRule/Quantity")]
        public ActionResult<Response<int>> CreateStoreQuantityRule([Required][FromQuery]string identifier, [FromRoute] int storeId, [FromBody] QuantityRuleDto rule)
        {
            if(!rule.IsValid()) return BadRequest("rule must have a subject");
            Response<int> response = _marketService.AddQuantityRule(identifier, storeId, rule.Subject, rule.MinQuantity, rule.MaxQuantity);
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.BadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<int>.OkResponse(response.Value));
            }
        }         

        [HttpPost]
        [Route("Store/{storeId}/AddRule/TotalPrice")]
        public ActionResult<Response<int>> CreateStoreTotalPriceRule([Required][FromQuery]string identifier, [FromRoute] int storeId, [FromBody] TotalPriceRuleDto rule)
        {
            if(!rule.IsValid()) return BadRequest("rule must have a subject");
            Response<int> response = _marketService.AddTotalPriceRule(identifier, storeId, rule.Subject, rule.TargetPrice);
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.BadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<int>.OkResponse(response.Value));
            }
        } 

        [HttpPost]
        [Route("Store/{storeId}/AddRule/CompositeRule")]
        public ActionResult<Response<int>> CreateStoreCompositeRule([Required][FromQuery]string identifier, [FromRoute] int storeId, [FromBody] CompositeRuleDto rule)
        {
            if(!rule.IsValid()) return BadRequest("composite rule must have rules");
            Response<int> response = _marketService.AddCompositeRule(identifier, storeId, rule.Operator, rule.Rules);
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
        [Route("Store/{storeId}/Info")]
        public ActionResult<Response<string>> GetInfo([Required][FromRoute] int storeId)
        {            
            Response<string> response = _marketService.GetInfo(storeId);
            if (response.ErrorOccured)
            {
                var getinforesponse = new ServerResponse<string>
                {
                    ErrorMessage = response.ErrorMessage,
                };
                return BadRequest(getinforesponse);
            }
            else
            {
                var getinforesponse = new ServerResponse<string>
                {
                    Value = response.Value,
                };
                return Ok(getinforesponse);
            }
        }

        [HttpGet]
        [Route("Store/{storeId}/Product/{productId}")]
        public ActionResult<Response<string>> GetProductInfo([Required][FromRoute] int storeId, [FromRoute] int productId)
        {            
            Response<string> response = _marketService.GetProductInfo(storeId, productId);
            if (response.ErrorOccured)
            {
                var getinforesponse = new ServerResponse<string>
                {
                    ErrorMessage = response.ErrorMessage,
                };
                return BadRequest(getinforesponse);
            }
            else
            {
                var getinforesponse = new ServerResponse<string>
                {
                    Value = response.Value,
                };
                return Ok(getinforesponse);
            }
        }

        [HttpPost]
        [Route("Store/{storeId}/Product/{productId}/KeyWord")]
        public async Task<ObjectResult> AddKeyWord([Required][FromQuery]string identifier, [FromQuery] string keyWord, [FromRoute] int storeId, [FromRoute] int productId)
        {
            Response response = await Task.Run(() => _marketService.AddKeyWord(identifier, keyWord, storeId, productId));
            if (response.ErrorOccured)
            {
                var addKeyWordResponse = new ServerResponse<string>
                {
                    ErrorMessage = response.ErrorMessage,
                };
                return BadRequest(addKeyWordResponse);
            }
            else
            {
                var addKeyWordResponse = new ServerResponse<string>
                {
                    Value = "add ket word success",
                };
                return Ok(addKeyWordResponse);
            }
        }   

        [HttpGet]
        [Route("Stores")]
        public async Task<ObjectResult> GetStores()
        {
            Response<List<StoreResultDto>> response = await Task.Run(() => _marketService.GetStores());
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.BadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<List<StoreResultDto>>.OkResponse(response.Value));
            }
        }
    }
}

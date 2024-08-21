using Newtonsoft.Json.Linq;
using EcommerceAPI.Controllers;
using EcommerceAPI.Models.Dtos;
using MarketBackend.Services.Interfaces;
using MarketBackend.Domain.Shipping;
using MarketBackend.Domain.Payment;
using MarketBackend.Services;
using System.Transactions;
using MarketBackend.DAL;
using MarketBackend.DAL.DTO;

namespace EcommerceAPI.initialize;

public class SceanarioParser
{
    private string adminUserName;
    private string adminPassword;
    private int Identifiers = 0;
    private IMarketService _marketService;
    private IClientService _clientService;
    string defaultPath = "initialize\\initialState.json";

    private string PATH;

    public SceanarioParser(IMarketService marketService, IClientService clientService)
    {
        this._clientService = clientService;
        this._marketService = marketService;

    }

    public async Task defaultParse (){
        DBcontext.GetInstance().Dispose();
        await Parse(defaultPath);
    }

    public async Task Parse(string path)
    {       
            try
            {
                string textJson = await File.ReadAllTextAsync(path);
                JObject scenarios = JObject.Parse(textJson);
                JArray sceanarios = (JArray)scenarios["Scenarios"];
                foreach (var sceanario in sceanarios.ToList())
                {
                    JObject parsedSeacnarios = JObject.Parse(sceanario.ToString());
                    string task = parsedSeacnarios["Scenario"]!.ToString();
                    ParseUseCase((Sceanarios)Enum.Parse(typeof(Sceanarios), task), parsedSeacnarios).Wait();
                }
            }
            catch (Exception ex)
            {
                StoreRepositoryRAM.Dispose();
                BasketRepositoryRAM.Dispose();
                ClientRepositoryRAM.Dispose();
                ProductRepositoryRAM.Dispose();
                RoleRepositoryRAM.Dispose();
                StoreRepositoryRAM.Dispose();
                PurchaseRepositoryRAM.Dispose();
                DBcontext.GetInstance().Dispose();

                MyLogger.GetLogger().Error(ex.Message);
            }
    }

    private async Task ParseUseCase(Sceanarios task, JObject usecaseJson)
    {
        switch (task)
        {
            case Sceanarios.Login:
            {
                var clientDto = new ClientDto
                {
                    Username = usecaseJson["Username"].ToString(),
                    Password = usecaseJson["Password"].ToString()
                };
                var res = _clientService.LoginClient(clientDto.Username, clientDto.Password);
                if (res.ErrorOccured)
                    throw new Exception("Failed to parse the login " + res.ErrorMessage);
                break;
            }
            case Sceanarios.Logout:
            {
                var res = _clientService.LogoutClient(
                    _clientService.GetTokenByUserName(usecaseJson["Username"].ToString()).Value);
                if (res.ErrorOccured)
                    throw new Exception(res.ErrorMessage);
                break;
            }
            case Sceanarios.EnterAsGuest:
            {
                var res = _clientService.EnterAsGuest(usecaseJson["Identifier"].ToString());
                if (res.ErrorOccured)
                    throw new Exception(res.ErrorMessage);                
                break;
            }
            case Sceanarios.Register:
            {
                var extendedClientDto = new ExtendedClientDto
                {
                    Username = usecaseJson["Username"].ToString(),
                    Password = usecaseJson["Password"].ToString(),
                    Email = usecaseJson["Email"].ToString(),
                    Age = int.Parse(usecaseJson["Age"].ToString())
                };
                var res = _clientService.Register(extendedClientDto.Username, extendedClientDto.Password, extendedClientDto.Email, extendedClientDto.Age);
                if (res.ErrorOccured)
                    throw new Exception(res.ErrorMessage);                
                break;
            }
            case Sceanarios.RegisterAsSystemAdmin:
            {
                var extendedClientDto = new ExtendedClientDto
                {
                    Username = usecaseJson["Username"].ToString(),
                    Password = usecaseJson["Password"].ToString(),
                    Email = usecaseJson["Email"].ToString(),
                    Age = int.Parse(usecaseJson["Age"].ToString())
                };
                var res = _marketService.RegisterAsSystemAdmin(extendedClientDto.Username, extendedClientDto.Password, extendedClientDto.Email, extendedClientDto.Age);
                if (res.ErrorOccured)
                    throw new Exception(res.ErrorMessage);                
                break;
            }
            case Sceanarios.UpdateCart:
            {
                var productDto = new ProductDto
                {
                    StoreId = int.Parse(usecaseJson["StoreId"].ToString()),
                    Id = int.Parse(usecaseJson["Id"].ToString()),
                    ProductName = usecaseJson["ProductName"].ToString(),
                    Quantity = int.Parse(usecaseJson["Quantity"].ToString())
                };
                if(productDto.Quantity>0){
                    var res = _clientService.AddToCart(
                        _clientService.GetTokenByUserName(usecaseJson["Username"].ToString()).Value,
                        productDto.StoreId, (int)productDto.Id, productDto.Quantity);
                    if (res.ErrorOccured)
                        throw new Exception(res.ErrorMessage);                
                }else{
                    var res = _clientService.RemoveFromCart(
                        _clientService.GetTokenByUserName(usecaseJson["Username"].ToString()).Value,
                        productDto.StoreId, (int)productDto.Id, Math.Abs(productDto.Quantity));
                    if (res.ErrorOccured)
                        throw new Exception(res.ErrorMessage); 
                }
                break;
            }
            case Sceanarios.ExitGuest:
            {
                var res = _clientService.ExitGuest(
                    usecaseJson["Identifier"].ToString());
                if (res.ErrorOccured)
                    throw new Exception(res.ErrorMessage);                
                break;
            }
            case Sceanarios.CreateStore:
            {
                var storeDto = new StoreDto
                {
                    Name = usecaseJson["Name"].ToString(),
                    Email = usecaseJson["Email"].ToString(),
                    PhoneNum = usecaseJson["PhoneNum"].ToString()
                };
                var res = _clientService.CreateStore(
                    _clientService.GetTokenByUserName(usecaseJson["Username"].ToString()).Value,
                    storeDto.Name, storeDto.Email, storeDto.PhoneNum);
                if (res.ErrorOccured)
                    throw new Exception(res.ErrorMessage);                
                break;
            }
            case Sceanarios.AddProduct:
            {
                var productDto = new ProductDto
                {
                    StoreId = int.Parse(usecaseJson["StoreId"].ToString()),
                    ProductName = usecaseJson["ProductName"].ToString(),
                    ProductDescription = usecaseJson["Description"].ToString(),
                    Price = double.Parse(usecaseJson["Price"].ToString()),
                    Quantity = int.Parse(usecaseJson["Quantity"].ToString()),
                    Category = usecaseJson["Category"].ToString(),
                    SellMethod = usecaseJson["SellMethod"].ToString()

                };
                var res = _marketService.AddProduct(
                    productDto.StoreId,
                    _clientService.GetTokenByUserName(usecaseJson["Username"].ToString()).Value,
                    productDto.ProductName,
                    productDto.SellMethod,
                    productDto.ProductDescription,
                    (double)productDto.Price,
                    productDto.Category,
                    productDto.Quantity,
                    productDto.AgeLimit);
                if (res.ErrorOccured)
                {                
                    MyLogger.GetLogger().Info(res.ErrorMessage);
                    throw new Exception(res.ErrorMessage);   
                }             
                break;
            }
            case Sceanarios.UpdateProduct:
            {
                var productDto = new ProductDto
                {
                    StoreId = int.Parse(usecaseJson["StoreId"].ToString()),
                    Id = int.Parse(usecaseJson["ProductId"].ToString()),
                    ProductDescription = usecaseJson["Description"].ToString(),
                    Price = double.Parse(usecaseJson["Price"].ToString()),
                    Quantity = int.Parse(usecaseJson["Quantity"].ToString()),
                    Category = usecaseJson["Category"].ToString(),
                    SellMethod = usecaseJson["SellMethod"].ToString()

                };
                var res = _marketService.UpdateProductQuantity(
                    productDto.StoreId,
                    _clientService.GetTokenByUserName(usecaseJson["Username"].ToString()).Value,
                    (int)productDto.Id,                    
                    productDto.Quantity);
                if (res.ErrorOccured)
                {                
                    MyLogger.GetLogger().Info(res.ErrorMessage);
                    throw new Exception(res.ErrorMessage);   
                }                   
                res = _marketService.UpdateProductPrice(
                    productDto.StoreId,
                    _clientService.GetTokenByUserName(usecaseJson["Username"].ToString()).Value,
                    (int)productDto.Id,                    
                    (double)productDto.Price);
                if (res.ErrorOccured)
                {                
                    MyLogger.GetLogger().Info(res.ErrorMessage);
                    throw new Exception(res.ErrorMessage);   
                }                   
                break;
            }            
            case Sceanarios.RemoveProduct:
            {
                var productDto = new ProductDto
                {
                    StoreId = int.Parse(usecaseJson["StoreId"].ToString()),
                    ProductName = usecaseJson["ProductName"].ToString(),
                    Id = int.Parse(usecaseJson["Id"].ToString())
                };
                var res = _marketService.RemoveProduct(
                    productDto.StoreId,
                    _clientService.GetTokenByUserName(usecaseJson["Username"].ToString()).Value,
                    (int)productDto.Id);
                if (res.ErrorOccured)
                {                
                    MyLogger.GetLogger().Info(res.ErrorMessage);
                    throw new Exception(res.ErrorMessage);   
                }                   
                break;
            }           
            case Sceanarios.AddStaff:
            {
                var staffMemberDto = new StaffMemberDto
                {
                    MemberUserName = usecaseJson["Username"].ToString(),
                    RoleName = usecaseJson["Role"].ToString()
                };
                var res = _marketService.AddStaffMember(
                    int.Parse(usecaseJson["StoreId"].ToString()),
                    _clientService.GetTokenByUserName(usecaseJson["Username"].ToString()).Value,
                    staffMemberDto.RoleName,
                    staffMemberDto.MemberUserName);
                if (res.ErrorOccured)
                {                
                    MyLogger.GetLogger().Info(res.ErrorMessage);
                    throw new Exception(res.ErrorMessage);   
                }                   
                break;
            }
            case Sceanarios.RemoveStaff:
            {
                var staffMemberDto = new StaffMemberDto
                {
                    MemberUserName = usecaseJson["MemberUserName"].ToString()
                };
                var res = _marketService.RemoveStaffMember(
                    int.Parse(usecaseJson["StoreId"].ToString()),
                    _clientService.GetTokenByUserName(usecaseJson["Username"].ToString()).Value,
                    staffMemberDto.MemberUserName);        
                if (res.ErrorOccured)
                {                
                    MyLogger.GetLogger().Info(res.ErrorMessage);
                    throw new Exception(res.ErrorMessage);   
                }                   
                break;
            }
            case Sceanarios.AddPermission:
            {
                var staffMemberDto = new StaffMemberDto
                {
                    MemberUserName = usecaseJson["MemberUserName"].ToString(),
                    Permission = usecaseJson["Permission"].ToObject<List<string>>()
                };
                var responses = new List<Response>();
                foreach(var permission in staffMemberDto.Permission){
                    var res = _marketService.AddPermission(
                        _clientService.GetTokenByUserName(usecaseJson["Username"].ToString()).Value,
                        int.Parse(usecaseJson["StoreId"].ToString()),
                        staffMemberDto.MemberUserName,
                        permission);
                    responses.Add(res);
                }
                if (responses.Any(res => res.ErrorOccured))
                {
                    throw new Exception(responses.Where(res => res.ErrorOccured).FirstOrDefault().ErrorMessage);
                }              
                break;
            }
            case Sceanarios.RemovePermission:
            {
                var staffMemberDto = new StaffMemberDto
                {
                    MemberUserName = usecaseJson["MemberUserName"].ToString(),
                    Permission = usecaseJson["Permission"].ToObject<List<string>>()
                };
                var responses = new List<Response>();
                foreach(var permission in staffMemberDto.Permission){
                    var res = _marketService.RemovePermission(
                        _clientService.GetTokenByUserName(usecaseJson["Username"].ToString()).Value,
                        int.Parse(usecaseJson["StoreId"].ToString()),
                        staffMemberDto.MemberUserName,
                        permission);
                    responses.Add(res);
                }
                if (responses.Any(res => res.ErrorOccured))
                {
                    throw new Exception(responses.Where(res => res.ErrorOccured).FirstOrDefault().ErrorMessage);
                }              
                break;
            }
            case Sceanarios.CloseStore:
            {
                var res = _marketService.CloseStore(
                    _clientService.GetTokenByUserName(usecaseJson["Username"].ToString()).Value,
                    int.Parse(usecaseJson["StoreId"].ToString()));
                if (res.ErrorOccured)
                {                
                    MyLogger.GetLogger().Info(res.ErrorMessage);
                    throw new Exception(res.ErrorMessage);   
                }                   
                break;
            }
            case Sceanarios.OpenStore:
            {
                var res = _marketService.OpenStore(
                    _clientService.GetTokenByUserName(usecaseJson["Username"].ToString()).Value,
                    int.Parse(usecaseJson["StoreId"].ToString()));
                if (res.ErrorOccured)
                {                
                    MyLogger.GetLogger().Info(res.ErrorMessage);
                    throw new Exception(res.ErrorMessage);   
                }                   
                break;
            }
            case Sceanarios.PurchaseCart:
            {
                var purchaseDto = new PurchaseDto
                {
                    StoreId = int.Parse(usecaseJson["StoreId"].ToString()),
                    ZipCode = usecaseJson["ZipCode"].ToString(),
                    Country = usecaseJson["Country"].ToString(),
                    City = usecaseJson["City"].ToString(),
                    Address = usecaseJson["Address"].ToString(),
                    HolderId = usecaseJson["HolderId"].ToString(),
                    CVV = usecaseJson["CVV"].ToString(),
                    CardHolder = usecaseJson["CardHolder"].ToString(),
                    ExpYear = usecaseJson["ExpYear"].ToString(),
                    ExpMonth = usecaseJson["ExpMonth"].ToString(),
                    CardNumber = usecaseJson["CardNumber"].ToString()
                };
                var res = _marketService.PurchaseCart(
                    _clientService.GetTokenByUserName(usecaseJson["Username"].ToString()).Value,
                    purchaseDto.PaymentInfo(),
                    purchaseDto.ShippingInfo());
                if (res.ErrorOccured)
                {                
                    MyLogger.GetLogger().Info(res.ErrorMessage);
                    throw new Exception(res.ErrorMessage);   
                }                   
                break;
            }
            case Sceanarios.SearchByKeywords:
            {
                var keywords = usecaseJson["Keywords"].ToObject<List<string>>();
                var responses = new List<Response>();
                foreach(var keyword in keywords){
                    var res = _marketService.SearchByKeywords(
                        keyword);
                    responses.Add(res);
                }
                if (responses.Any(res => res.ErrorOccured))
                {
                    throw new Exception(responses.Where(res => res.ErrorOccured).FirstOrDefault().ErrorMessage);
                }              
                break;
            }
            case Sceanarios.SearchByNames:
            {
                var names = usecaseJson["Names"].ToObject<List<string>>();
                var responses = new List<Response>();
                foreach(var name in names){
                    var res = _marketService.SearchByName(name);
                    responses.Add(res);
                }
                if (responses.Any(res => res.ErrorOccured))
                {
                    throw new Exception(responses.Where(res => res.ErrorOccured).FirstOrDefault().ErrorMessage);
                }              
                break;
            }
            case Sceanarios.SearchByCategory:
            {
                var categories = usecaseJson["Categories"].ToObject<List<string>>();
                var responses = new List<Response>();
                foreach(var category in categories){
                    var res = _marketService.SearchByCategory(category);
                    responses.Add(res);
                }
                if (responses.Any(res => res.ErrorOccured))
                {
                    throw new Exception(responses.Where(res => res.ErrorOccured).FirstOrDefault().ErrorMessage);
                }              
                break;                
            }
            case Sceanarios.GetStoreById:
            {
                var res = _marketService.GetStoreById(
                    int.Parse(usecaseJson["StoreId"].ToString()));
                if (res.ErrorOccured)
                {                
                    MyLogger.GetLogger().Info(res.ErrorMessage);
                    throw new Exception(res.ErrorMessage);   
                }                   
                break;
            }
            case Sceanarios.ShowShopPurchaseHistory:
            {
                var res = _marketService.GetPurchaseHistoryByStore(
                    int.Parse(usecaseJson["StoreId"].ToString()),
                    _clientService.GetTokenByUserName(usecaseJson["Username"].ToString()).Value
                    );
                if (res.ErrorOccured)
                {                
                    MyLogger.GetLogger().Info(res.ErrorMessage);
                    throw new Exception(res.ErrorMessage);   
                }                   
                break;
            }
            case Sceanarios.GetStoreDiscountPolicies:
            {
                var res = _marketService.GetStoreDiscountPolicies(
                    int.Parse(usecaseJson["StoreId"].ToString()),
                    _clientService.GetTokenByUserName(usecaseJson["Username"].ToString()).Value
                    );
                if (res.ErrorOccured)
                {                
                    MyLogger.GetLogger().Info(res.ErrorMessage);
                    throw new Exception(res.ErrorMessage);   
                }                   
                break;
            }
            case Sceanarios.CreateStoreDiscountPolicy:
            {
                var policyDto = new DiscountPolicyDto
                {
                    ExpirationDate = DateTime.Parse(usecaseJson["ExpirationDate"].ToString()),
                    Subject = usecaseJson["Subject"].ToString(),
                    RuleId = int.Parse(usecaseJson["RuleId"].ToString()),
                    Precantage = double.Parse(usecaseJson["Precantage"].ToString())
                };
                var res = _marketService.AddDiscountPolicy(
                    _clientService.GetTokenByUserName(usecaseJson["Username"].ToString()).Value,
                    int.Parse(usecaseJson["StoreId"].ToString()),
                    policyDto.ExpirationDate,
                    policyDto.Subject,
                    policyDto.RuleId,
                    policyDto.Precantage);
                if (res.ErrorOccured)
                {                
                    MyLogger.GetLogger().Info(res.ErrorMessage);
                    throw new Exception(res.ErrorMessage);   
                }                   
                break;
            }
            case Sceanarios.CreateStorePolicy:
            {
                var policyDto = new PolicyDto
                {
                    ExpirationDate = DateTime.Parse(usecaseJson["ExpirationDate"].ToString()),
                    Subject = usecaseJson["Subject"].ToString(),
                    RuleId = int.Parse(usecaseJson["RuleId"].ToString())
                };
                var res = _marketService.AddPurchasePolicy(
                    _clientService.GetTokenByUserName(usecaseJson["Username"].ToString()).Value,
                    int.Parse(usecaseJson["StoreId"].ToString()),
                    policyDto.ExpirationDate,
                    policyDto.Subject,
                    policyDto.RuleId);
                if (res.ErrorOccured)
                {                
                    MyLogger.GetLogger().Info(res.ErrorMessage);
                    throw new Exception(res.ErrorMessage);   
                }                   
                break;
            }
            case Sceanarios.CreateStoreCompositePolicy:
            {
                var compositePolicyDto = new CompositePolicyDto
                {
                    ExpirationDate = DateTime.Parse(usecaseJson["ExpirationDate"].ToString()),
                    Subject = usecaseJson["Subject"].ToString(),
                    Operator = int.Parse(usecaseJson["Operator"].ToString()),
                    Policies = usecaseJson["Policies"].ToObject<List<int>>()
                };
                var res = _marketService.AddCompositePolicy(
                    _clientService.GetTokenByUserName(usecaseJson["Username"].ToString()).Value,
                    int.Parse(usecaseJson["StoreId"].ToString()),
                    compositePolicyDto.ExpirationDate,
                    compositePolicyDto.Subject,
                    compositePolicyDto.Operator,
                    compositePolicyDto.Policies);
                if (res.ErrorOccured)
                {                
                    MyLogger.GetLogger().Info(res.ErrorMessage);
                    throw new Exception(res.ErrorMessage);   
                }                   
                break;
            }            
            case Sceanarios.CreateStoreRule:
            {
                var ruleDto = new RuleDto
                {
                    Subject = usecaseJson["Subject"].ToString()
                };
                var res = _marketService.AddSimpleRule(
                    _clientService.GetTokenByUserName(usecaseJson["Username"].ToString()).Value,
                    int.Parse(usecaseJson["StoreId"].ToString()),
                    ruleDto.Subject);
                if (res.ErrorOccured)
                {                
                    MyLogger.GetLogger().Info(res.ErrorMessage);
                    throw new Exception(res.ErrorMessage);   
                }                   
                break;
            }
            case Sceanarios.CreateStoreQuantityRule:
            {
                var quantityRuleDto = new QuantityRuleDto
                {
                    Subject = usecaseJson["Subject"].ToString(),
                    MinQuantity = int.Parse(usecaseJson["MinQuantity"].ToString()),
                    MaxQuantity = int.Parse(usecaseJson["MaxQuantity"].ToString())
                };
                var res = _marketService.AddQuantityRule(
                    _clientService.GetTokenByUserName(usecaseJson["Username"].ToString()).Value,
                    int.Parse(usecaseJson["StoreId"].ToString()),
                    quantityRuleDto.Subject,
                    quantityRuleDto.MinQuantity,
                    quantityRuleDto.MaxQuantity);
                if (res.ErrorOccured)
                {                
                    MyLogger.GetLogger().Info(res.ErrorMessage);
                    throw new Exception(res.ErrorMessage);   
                }                   
                break;
            }
            case Sceanarios.CreateStoreTotalPriceRule:
            {
                var totalPriceRuleDto = new TotalPriceRuleDto
                {
                    Subject = usecaseJson["Subject"].ToString(),
                    TargetPrice = int.Parse(usecaseJson["TargetPrice"].ToString())
                };
                var res = _marketService.AddTotalPriceRule(
                    _clientService.GetTokenByUserName(usecaseJson["Username"].ToString()).Value,
                    int.Parse(usecaseJson["StoreId"].ToString()),
                    totalPriceRuleDto.Subject,
                    totalPriceRuleDto.TargetPrice);
                if (res.ErrorOccured)
                {                
                    MyLogger.GetLogger().Info(res.ErrorMessage);
                    throw new Exception(res.ErrorMessage);   
                }                   
                break;
            }
            case Sceanarios.CreateStoreCompositeRule:
            {
                var compositeRuleDto = new CompositeRuleDto
                {
                    Operator = int.Parse(usecaseJson["Operator"].ToString()),
                    Rules = usecaseJson["Rules"].ToObject<List<int>>()
                };
                var res = _marketService.AddCompositeRule(
                    _clientService.GetTokenByUserName(usecaseJson["Username"].ToString()).Value,
                    int.Parse(usecaseJson["StoreId"].ToString()),
                    compositeRuleDto.Operator,
                    compositeRuleDto.Rules);
                if (res.ErrorOccured)
                {                
                    MyLogger.GetLogger().Info(res.ErrorMessage);
                    throw new Exception(res.ErrorMessage);   
                }                   
                break;
            }
            case Sceanarios.GetInfo:
            {
                var res = _marketService.GetInfo(
                    int.Parse(usecaseJson["StoreId"].ToString()));
                if (res.ErrorOccured)
                {                
                    MyLogger.GetLogger().Info(res.ErrorMessage);
                    throw new Exception(res.ErrorMessage);   
                }                   
                break;
            }
            case Sceanarios.GetProductInfo:
            {
                var res = _marketService.GetProductInfo(
                    int.Parse(usecaseJson["StoreId"].ToString()),
                    int.Parse(usecaseJson["ProductId"].ToString()));
                if (res.ErrorOccured)
                {                
                    MyLogger.GetLogger().Info(res.ErrorMessage);
                    throw new Exception(res.ErrorMessage);   
                }                   
                break;
            }
            case Sceanarios.AddKeyWord:
            {
                var res = _marketService.AddKeyWord(
                    _clientService.GetTokenByUserName(usecaseJson["Username"].ToString()).Value,
                    usecaseJson["KeyWord"].ToString(),
                    int.Parse(usecaseJson["StoreId"].ToString()),
                    int.Parse(usecaseJson["ProductId"].ToString()));
                if (res.ErrorOccured)
                {                
                    MyLogger.GetLogger().Info(res.ErrorMessage);
                    throw new Exception(res.ErrorMessage);   
                }                   
                break;
            }
            default:
                throw new Exception("Unsupported task in the InitFile ");
        }
    }
}

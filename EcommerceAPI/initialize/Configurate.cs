using Newtonsoft.Json.Linq;
using EcommerceAPI.Controllers;
using EcommerceAPI.Models.Dtos;
using MarketBackend.Services.Interfaces;
using MarketBackend.Domain.Shipping;
using MarketBackend.Domain.Payment;
using MarketBackend.Services;
using MarketBackend.DAL.DTO;
using System.Transactions;
using MarketBackend.DAL;
using MarketBackend.Domain.Market_Client;

namespace EcommerceAPI.initialize;

public class Configurate
{
    private readonly IMarketService _service;
    private readonly IClientService _clientService;


    public Configurate(IMarketService service,  IClientService clientService)
    {
        _service = service;
        _clientService = clientService;
    }

    public string Parse(string PATH = null)
    {
        PATH ??= Path.Combine(Environment.CurrentDirectory, "initialize\\config.json");        

        string textJson = "";
        try
        {
            textJson = File.ReadAllText(PATH);
        }
        catch (Exception e)
        {            
            MyLogger.GetLogger().Info("open initializing file fail");
            throw new Exception("open initializing file fail");
        }
        JObject scenarioDtoDict = JObject.Parse(textJson);
        try
        {
            if (scenarioDtoDict["Test"].Value<bool>())
            {
                MyLogger.GetLogger().Info("configured test DB");
                DBcontext.SetTestDB();
            }
            else if (scenarioDtoDict["Local"].Value<bool>())
            {
                MyLogger.GetLogger().Info("configured local DB");
                DBcontext.SetLocalDB();
            }
            else
            {
                MyLogger.GetLogger().Info("configured remote DB");
                DBcontext.SetRemoteDB();
            }

            if (scenarioDtoDict["Initialize"].Value<string>() == InitializeOptions.File.GetDescription())
            {                
                string initPATH = Path.Combine(Environment.CurrentDirectory, "initialize\\" + scenarioDtoDict["InitialState"]);
                DBcontext.GetInstance().Dispose();
                StoreRepositoryRAM.Dispose();
                BasketRepositoryRAM.Dispose();
                ClientRepositoryRAM.Dispose();
                ProductRepositoryRAM.Dispose();
                RoleRepositoryRAM.Dispose();
                StoreRepositoryRAM.Dispose();
                PurchaseRepositoryRAM.Dispose();
                ClientManager.GetInstance().Reset();               
                try{
                    new SceanarioParser(_service, _clientService).Parse(initPATH).Wait();
                    MyLogger.GetLogger().Info("Initialize from file");

                }catch(Exception){
                    MyLogger.GetLogger().Info("Initialize from file failed");
                }
            }else if(scenarioDtoDict["Initialize"].Value<string>() == InitializeOptions.Empty.GetDescription()){
                DBcontext.GetInstance().Dispose();
                StoreRepositoryRAM.Dispose();
                BasketRepositoryRAM.Dispose();
                ClientRepositoryRAM.Dispose();
                ProductRepositoryRAM.Dispose();
                RoleRepositoryRAM.Dispose();
                StoreRepositoryRAM.Dispose();
                PurchaseRepositoryRAM.Dispose(); 
                ClientManager.GetInstance().Reset();               
                _service.RegisterAsSystemAdmin(scenarioDtoDict["AdminUsername"].Value<string>(), scenarioDtoDict["AdminPassword"].Value<string>(), scenarioDtoDict["AdminEmail"].Value<string>(), scenarioDtoDict["AdminAge"].Value<int>());
                    MyLogger.GetLogger().Info("Initialize empty");
            }else{
                    MyLogger.GetLogger().Info("Initialize from DB");
            }
            
            var port = scenarioDtoDict["Port"].ToString();
            return port;
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as necessary
            MyLogger.GetLogger().Info("Failed during configuration");
            throw new Exception("Failed during configuration", ex);
        }
    }

    public static bool VerifyJsonStructure(string filePath)
    {
        string expectedJson = @"
        {        
            ""AdminUsername"": ""string"",
            ""AdminPassword"": ""string"",
            ""InitialState"": ""string"",
            ""Port"": 0,
            ""ExternalServices"": false,        
            ""Local"": false,
            ""Initialize"": true
        }";

        JObject expectedObject = JObject.Parse(expectedJson);
        JObject actualObject = JObject.Parse(System.IO.File.ReadAllText(filePath));

        foreach (var property in expectedObject.Properties())
        {
            if (!actualObject.ContainsKey(property.Name) ||
                actualObject[property.Name].Type != GetJTokenType(property.Value))
            {
                return false;
            }
        }

        return true;
    }

    private static JTokenType GetJTokenType(JToken value)
    {
        if (value.Type == JTokenType.String)
        {
            return JTokenType.String;
        }
        else if (value.Type == JTokenType.Boolean)
        {
            return JTokenType.Boolean;
        }
        else if (value.Type == JTokenType.Integer || value.Type == JTokenType.Float)
        {
            return JTokenType.Integer;
        }

        return JTokenType.Null;
    }
}

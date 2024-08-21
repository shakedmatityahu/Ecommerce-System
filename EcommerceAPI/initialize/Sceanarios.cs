using System.ComponentModel;

namespace EcommerceAPI.initialize
{

    public enum Sceanarios
    {
        // ClientController API calls
        [Description("Login")]
        Login,

        [Description("Logout")]
        Logout,

        [Description("EnterAsGuest")]
        EnterAsGuest,

        [Description("Register")]
        Register,
        
        [Description("RegisterAsSystemAdmin")]
        RegisterAsSystemAdmin,

        [Description("UpdateCart")]
        UpdateCart,

        [Description("ExitGuest")]
        ExitGuest,

        // MarketController API calls
        [Description("AddStaff")]
        AddStaff,

        [Description("RemoveStaff")]
        RemoveStaff,

        [Description("AddPermission")]
        AddPermission,

        [Description("RemovePermission")]
        RemovePermission,

        [Description("CloseStore")]
        CloseStore,

        [Description("OpenStore")]
        OpenStore,
        
        [Description("CreateStore")]
        CreateStore,

        [Description("PurchaseCart")]
        PurchaseCart,

        [Description("RemoveProduct")]
        RemoveProduct,

        [Description("AddProduct")]
        AddProduct,

        [Description("UpdateProduct")]
        UpdateProduct,

        [Description("SearchByKeywords")]
        SearchByKeywords,

        [Description("SearchByNames")]
        SearchByNames,

        [Description("SearchByCategory")]
        SearchByCategory,

        [Description("GetStoreById")]
        GetStoreById,

        [Description("ShowShopPurchaseHistory")]
        ShowShopPurchaseHistory,

        [Description("GetStoreDiscountPolicies")]
        GetStoreDiscountPolicies,

        [Description("GetStorePurchacePolicy")]
        GetStorePurchacePolicy,

        [Description("CreateStoreDiscountPolicy")]
        CreateStoreDiscountPolicy,

        [Description("CreateStorePolicy")]
        CreateStorePolicy,

        [Description("CreateStoreCompositePolicy")]
        CreateStoreCompositePolicy,

        [Description("RemoveStorePolicy")]
        RemoveStorePolicy,

        [Description("RemoveStoreDiscountPolicy")]
        RemoveStoreDiscountPolicy,

        [Description("GetStoreRules")]
        GetStoreRules,

        [Description("CreateStoreRule")]
        CreateStoreRule,

        [Description("CreateStoreQuantityRule")]
        CreateStoreQuantityRule,

        [Description("CreateStoreTotalPriceRule")]
        CreateStoreTotalPriceRule,

        [Description("CreateStoreCompositeRule")]
        CreateStoreCompositeRule,

        [Description("GetInfo")]
        GetInfo,

        [Description("GetProductInfo")]
        GetProductInfo,

        [Description("AddKeyWord")]
        AddKeyWord,

        // Other API calls
        [Description("OtherAPI")]
        OtherAPI
    }

    public static class APIEndpointsExtensions
    {
        public static string GetDescription(this Sceanarios apiEndpoint)
        {
            var field = apiEndpoint.GetType().GetField(apiEndpoint.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            return attribute == null ? apiEndpoint.ToString() : attribute.Description;
        }
    }

}
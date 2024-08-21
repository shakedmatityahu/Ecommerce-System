using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.ComponentModel; 
using System.Threading.Tasks;

namespace MarketBackend.Domain.Market_Client
{
    public enum RoleName
    {
        [Description("Founder")]
        Founder,
        
        [Description("Owner")]
        Owner,
        
        [Description("Manager")]
        Manager
    }

    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? value.ToString();
        }
    }
}

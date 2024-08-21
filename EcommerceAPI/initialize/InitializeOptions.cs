using System.ComponentModel;

namespace EcommerceAPI.initialize
{
    public enum InitializeOptions
    {
        [Description("DB")]
        DB,
        
        [Description("File")]
        File,

        [Description("Empty")]
        Empty
    }

    public static class InitDescExtention
    {
        public static string GetDescription(this InitializeOptions option)
        {
            var field = option.GetType().GetField(option.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            return attribute == null ? option.ToString() : attribute.Description;
        }
    }
}


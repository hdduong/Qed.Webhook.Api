using System;
using System.ComponentModel;
using System.Reflection;

namespace Qed.Webhook.Api.Shared.Enums
{
    public static class EnumHelper
    {
        public static string GetDescription(Enum @enum)
        {
            if (@enum == null)
                return null;

            string description = @enum.ToString();
            FieldInfo fi = @enum.GetType().GetField(@enum.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length > 0) description = attributes[0].Description;
           
            return description;
        }
    }
}

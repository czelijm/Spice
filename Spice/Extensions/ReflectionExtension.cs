using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Extensions
{

    public static class ReflectionExtension
    {
        public static string GetPropertyValueAsString<T>(this T item, string propertyName) 
        {
            return item.GetType().GetProperty(propertyName).GetValue(item,null).ToString();
        }
    }
}

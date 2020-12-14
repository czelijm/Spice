using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Extensions
{

    public static class IEnumerableExtension
    {
        public static IEnumerable<SelectListItem> ToSelectListItem<T>(this IEnumerable<T> items, Guid selectedValue)
        {
            return from item in items
                   select new SelectListItem
                   {
                       Text = item.GetPropertyValueAsString("Name"),
                       Value = item.GetPropertyValueAsString("Id"),
                       Selected = item.GetPropertyValueAsString("Id").Equals(selectedValue.ToString())
                   };
        }
    }
}
//1. Extension methods are static classes
//2. First argument must be type of the extended class 
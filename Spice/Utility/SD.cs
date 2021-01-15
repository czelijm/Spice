using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Utility
{
    public static class SD
    {
        public static string DefaultFoodImage = "default_food.png";
        
        public static string ImageDefaulInnerPath = @"images\"; //@ for verbatim mode

        public const string ManagerUser = "Manager";
        public static string KitchenUser = "Kitchen";
        public static string FrontDeskUser = "FrontDesk";
        public static string CustomerEndUser = "Customer";
        public static string[] UserArray = { ManagerUser, KitchenUser, FrontDeskUser, CustomerEndUser};

        public static string SessionCountCookie = "ssCount";
        public static string SessionCartCountCookie = "ssCartCount";

    }
}


/*
 
 class MyType { public int prop1 { get; } public string prop2 { get; } public int[] prop3 { get; } public int prop4 { get; } public string prop5 { get; } public string prop6 { get; } public List<string> GetAllPropertyValues() { List<string> values = new List<string>(); foreach (var pi in typeof(MyType).GetProperties()) { values.Add(pi.GetValue(this, null).ToString()); } return values; } }
 
 */






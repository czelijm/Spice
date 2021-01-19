using Spice.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
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

        //public static string SessionCountCookie = "ssCount";
        public static string SessionCartCountCookie = "ssCartCount";
        public static string SessionCouponCodeCookie = "ssCouponCode";

		public static string DefaultCultureTag = "en-US";
		public static CultureInfo DefaultCultureInfo = new CultureInfo(DefaultCultureTag);

		public static string ConvertToRawHtml(string source)
		{
			char[] array = new char[source.Length];
			int arrayIndex = 0;
			bool inside = false;

			for (int i = 0; i < source.Length; i++)
			{
				char let = source[i];
				if (let == '<')
				{
					inside = true;
					continue;
				}
				if (let == '>')
				{
					inside = false;
					continue;
				}
				if (!inside)
				{
					array[arrayIndex] = let;
					arrayIndex++;
				}
			}
			return new string(array, 0, arrayIndex);
		}

		public static decimal DicountedPrice(Coupon coupon, decimal orderTotalOriginal) 
		{
            if (coupon == null || ((decimal) coupon.MinimumAmmount )> orderTotalOriginal)
            {
				return orderTotalOriginal;
            }
			//decimal result = 0;
            if (Convert.ToInt32(coupon.CouponType)==(int)Coupon.ECouponType.Dollars)
            {
				//10$ of 100$
				orderTotalOriginal -= (decimal)coupon.Dsicount;
            }
            else if (Convert.ToInt32(coupon.CouponType) == (int)Coupon.ECouponType.Percent)
            {
				orderTotalOriginal -= (orderTotalOriginal * (decimal)coupon.Dsicount / 100); 
            }

			return Math.Round(orderTotalOriginal, 2);
			
		}

	}

}


/*
 
 class MyType { public int prop1 { get; } public string prop2 { get; } public int[] prop3 { get; } public int prop4 { get; } public string prop5 { get; } public string prop6 { get; } public List<string> GetAllPropertyValues() { List<string> values = new List<string>(); foreach (var pi in typeof(MyType).GetProperties()) { values.Add(pi.GetValue(this, null).ToString()); } return values; } }
 
 */






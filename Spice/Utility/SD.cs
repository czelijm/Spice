using Spice.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Spice.Utility
{
    public static class SD
    {
        public static string DefaultFoodImage = "default_food.png";
        
        public static string ImageDefaulInnerPath = @"images"+FileSeparator(); //@ for verbatim mode

        public const string ManagerUser = "Manager";
        public const string KitchenUser = "Kitchen";
        public const string FrontDeskUser = "FrontDesk";
        public const string CustomerEndUser = "Customer";
        public static string[] UserArray = { ManagerUser, KitchenUser, FrontDeskUser, CustomerEndUser};

        //public static string SessionCountCookie = "ssCount";
        public static string SessionCartCountCookie = "ssCartCount";
        public static string SessionCouponCodeCookie = "ssCouponCode";
        public static string SessionAllowCookie = "0";


		public static string DefaultCultureTag = "en-US";
		public static CultureInfo DefaultCultureInfo = new CultureInfo(DefaultCultureTag);

		public static string OpenStoreHourStringUS = "11:00 AM";
		public static string OpenStoreHourStringEU = "11:00";
		public static string CloseStoreHourStringUS = "9:00 PM";
		public static string CloseStoreHourStringEU = "21:00";
		public static string TimeIntervalTimePickerStringMinutes= "30";

		public static string TransactionDataName = "Spice Restaurant";
		public static string TransactionDataDescription = "11 Cool street, Order Charge";
		public static string TransactionDataImage = "https://stripe.com/img/documentation/checkout/marketplace.png";
		public static string TransactionCurrency = "usd";
		public static string TransactionSucceeded = "succeeded";

		public static int PageSize = 2;

        public static class Status 
		{
			public static string orderSubmitted = "Submitted";
			public static string orderInProcess = "Being Prepared";
			public static string orderReadyForPickup = "Ready for pickup";
			public static string orderCompleted = "Completed";
			public static string orderCanceled = "Canceled";
			
			public static string paymentPending  = "Pending";
			public static string paymentApproved = "Approved";
			public static string paymentRejected = "Rejected";
			
		}

		public static class Image
		{
			public static string orderPlaced = "/images/OrderPlaced.png";
			public static string orderReadyForPickUp = "/images/ReadyForPickup.png";
			public static string orderInKitchen = "/images/InKitchen.png";
			public static string orderCompleted = "/images/completed.png";

			public static Dictionary<String, String> Images = new Dictionary<string, string>();

			static Image() 
			{
				Images.Add(Status.orderSubmitted, orderPlaced);
				Images.Add(Status.orderInProcess, orderInKitchen);
				Images.Add(Status.orderReadyForPickup, orderReadyForPickUp);
				Images.Add(Status.orderCompleted, orderCompleted);
			}
			
		}

		public static class AdminAccountInfo 
		{
			public static string UserName = "Admin";
			public static string Name = "Admin Citizen";
			public static string Email = "";
			public static bool EmailConfirmed = true;
			public static string Phone = "";
			public static string Password = "";
		}

		public static class CompanyInformations
        {
			//public static string emailAdmin = "admin@spice.com";
			public static string emailAdmin = "";
			public static string Name = "Spice Restaurant";
			public static string emailSubject = "Spice - Order Created ";
			public static string emailSubjectStatusUpdated = "Spice - Order status updated ";
			public static string emailMessageOrderSubmitedSuccess = "Order has been submited to successfully";
			public static string emailMessageOrderStatutsUpdatedGeneric = "Order status updated, status: ";

		}
		public static class DBProvier 
		{
			public const string postgres = "postgres";
			public const string sqlServer = "sqlserver";
			public const string activeProviderSqlServer = "Microsoft.EntityFrameworkCore.SqlServer";
			public const string activeProviderPostgres = "Npgsql.EntityFrameworkCore.PostgreSQL";
			
		}


		public static bool IsWindows()
		{
			return System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
		}

		public static string FileSeparator()
		{
			return IsWindows() ? @"\" : @"/" ;
		}

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






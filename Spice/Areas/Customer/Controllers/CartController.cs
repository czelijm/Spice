﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Spice.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        [BindProperty]
        public OrderDetailsCartViewModel OrderDetailsCart { get; set; }

        public CartController(ApplicationDbContext db)
        {
            _db = db;
            //OrderDetailsCart = new OrderDetailsCartViewModel();
        }

        public async Task<IActionResult> Index()
        {
            OrderDetailsCart = new OrderDetailsCartViewModel()
            {
                OrderHeader = new Models.OrderHeader(),
            };

            OrderDetailsCart.OrderHeader.OrderTotalDiscount = "0";

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim==null)
            {
                return RedirectToAction(nameof(Index));
            }

            var cart = await _db.ShoppingCart.Where(c => c.ApplicationUserId.ToString().Equals(claim.Value)).ToListAsync();

            if (cart!=null)
            {
                OrderDetailsCart.ListCart = cart.ToList(); 
                    
            }

            foreach (var list in OrderDetailsCart.ListCart)
            {
                list.MenuItem = await _db.MenuItem.FirstOrDefaultAsync(m=>m.Id.ToString().Equals(list.MenuItemId.ToString()));
                decimal orderTotal = decimal.Parse(OrderDetailsCart.OrderHeader.OrderTotalDiscount);
                orderTotal+=(decimal)(list.MenuItem.Price*list.Count);
                OrderDetailsCart.OrderHeader.OrderTotalDiscount = orderTotal.ToString();
                list.MenuItem.Description = Utility.SD.ConvertToRawHtml(list.MenuItem.Description);
                if (list.MenuItem.Description.Length>100)
                {
                    list.MenuItem.Description = list.MenuItem.Description.Substring(0, 99) + "...";
                }
            }
            OrderDetailsCart.OrderHeader.OrderTotalOriginal = OrderDetailsCart.OrderHeader.OrderTotalDiscount;


            var couponCodeFormSession = HttpContext.Session.GetString(SD.SessionCouponCodeCookie);
            if (couponCodeFormSession!=null)
            {
                OrderDetailsCart.OrderHeader.CouponCode = couponCodeFormSession;
                var couponFromDb = await _db.Coupon.Where(c => c.Name.ToLower().Equals(couponCodeFormSession.ToLower())).FirstOrDefaultAsync();
                OrderDetailsCart.OrderHeader.OrderTotalDiscount =  SD.DicountedPrice(couponFromDb, decimal.Parse(OrderDetailsCart.OrderHeader.OrderTotalOriginal)).ToString();
            }

            return View(OrderDetailsCart);
        }
        
        public async Task<IActionResult> Summary()
        {
            OrderDetailsCart = new OrderDetailsCartViewModel()
            {
                OrderHeader = new Models.OrderHeader(),
            };

            OrderDetailsCart.OrderHeader.OrderTotalDiscount = "0";

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim==null)
            {
                return RedirectToAction(nameof(Index));
            }

            ApplicationUser appUser = await _db.ApplicationUser.Where(c => c.Id.Equals(claim.Value)).FirstOrDefaultAsync();

            var cart = await _db.ShoppingCart.Where(c => c.ApplicationUserId.ToString().Equals(claim.Value)).ToListAsync();

            if (cart!=null)
            {
                OrderDetailsCart.ListCart = cart.ToList(); 
                    
            }

            foreach (var list in OrderDetailsCart.ListCart)
            {
                list.MenuItem = await _db.MenuItem.FirstOrDefaultAsync(m=>m.Id.ToString().Equals(list.MenuItemId.ToString()));
                decimal orderTotal = decimal.Parse(OrderDetailsCart.OrderHeader.OrderTotalDiscount);
                orderTotal+=(decimal)(list.MenuItem.Price*list.Count);
                OrderDetailsCart.OrderHeader.OrderTotalDiscount = orderTotal.ToString();

            }
            OrderDetailsCart.OrderHeader.OrderTotalOriginal = OrderDetailsCart.OrderHeader.OrderTotalDiscount;
            OrderDetailsCart.OrderHeader.PickupName = appUser.FirstName + " " + appUser.LastName;
            OrderDetailsCart.OrderHeader.PhoneNumber = appUser.PhoneNumber;
            OrderDetailsCart.OrderHeader.PickupTime = DateTime.Now;


            var couponCodeFormSession = HttpContext.Session.GetString(SD.SessionCouponCodeCookie);
            if (couponCodeFormSession!=null)
            {
                OrderDetailsCart.OrderHeader.CouponCode = couponCodeFormSession;
                var couponFromDb = await _db.Coupon.Where(c => c.Name.ToLower().Equals(couponCodeFormSession.ToLower())).FirstOrDefaultAsync();
                OrderDetailsCart.OrderHeader.OrderTotalDiscount =  SD.DicountedPrice(couponFromDb, decimal.Parse(OrderDetailsCart.OrderHeader.OrderTotalOriginal)).ToString();
            }

            return View(OrderDetailsCart);
        }

        public async Task<IActionResult> AddCoupon()
        //retreive coupon and add to the User's session
        {

            OrderDetailsCart.OrderHeader.CouponCode ??= "";
            HttpContext.Session.SetString(SD.SessionCouponCodeCookie, OrderDetailsCart.OrderHeader.CouponCode);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> RemoveCoupon()
        //retreive coupon and add to the User's session
        {
            HttpContext.Session.SetString(SD.SessionCouponCodeCookie, string.Empty);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Minus(Guid cartId) 
        {

            if (cartId.Equals(Guid.Empty)) return NotFound();
            var result = await _db.ShoppingCart.FirstAsync(s=>s.Id.ToString().Equals(cartId.ToString()));
            if (result == null) return NotFound();
            result.Count = result.Count > 1 ? --result.Count: 1;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Plus(Guid cartId) 
        {

            if (cartId.Equals(Guid.Empty)) return NotFound();
            var result = await _db.ShoppingCart.FirstAsync(s=>s.Id.ToString().Equals(cartId.ToString()));
            if (result == null) return NotFound();
            result.Count++;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Remove(Guid cartId) 
        {

            if (cartId.Equals(Guid.Empty)) return NotFound();
            var result = await _db.ShoppingCart.FirstAsync(s=>s.Id.ToString().Equals(cartId.ToString()));
            if (result == null) return NotFound();
            _db.ShoppingCart.Remove(result);
            await _db.SaveChangesAsync();

            //decrease coockie item count
            var coockieCount = HttpContext.Session.GetInt32(SD.SessionCartCountCookie) - 1;
            HttpContext.Session.SetInt32(SD.SessionCartCountCookie, (int)coockieCount);

            return RedirectToAction(nameof(Index));
        }



        

    }
}
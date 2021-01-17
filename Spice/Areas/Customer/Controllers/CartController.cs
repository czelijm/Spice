using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;
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
        private OrderDetailsCartViewModel OrderDetailsCart { get; set; }

        public CartController(ApplicationDbContext db)
        {
            _db = db;
            OrderDetailsCart = new OrderDetailsCartViewModel();
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

            return View(OrderDetailsCart);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Spice.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public ShoppingCart ShoppingCart { get; set; }

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
           // ShoppingCart = new ShoppingCart();
        }

        public async Task<IActionResult> Index()
        {
            IndexViewModel indexViewModel = new IndexViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                MenuItemList = await _db.MenuItem.Include(s=>s.Category).Include(s=>s.SubCategory).ToListAsync(),
                CouponList = await _db.Coupon.Where(s=>s.IsActive==true).ToListAsync()
            };

            return View(indexViewModel);
        }


        [Authorize]
        public async Task<IActionResult> Details(Guid id) 
        {
            if (id == null) return NotFound();
            var menuItemFromDb = await _db.MenuItem.Include(m=>m.Category).Include(m=>m.SubCategory).FirstOrDefaultAsync(m => m.Id==id);
            if (menuItemFromDb == null) return NotFound();

            ShoppingCart shoppingCart = new ShoppingCart()
            {
                MenuItem = menuItemFromDb,
                MenuItemId = menuItemFromDb.Id,
            };

            return View(shoppingCart);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(ShoppingCart shoppingCartXD) 
        {

            if (ShoppingCart == null) return View(ShoppingCart);

            ShoppingCart.Id = new Guid();

            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                ShoppingCart.ApplicationUserId = new Guid(claim.Value);

                ShoppingCart shoppingCartFromDb = await _db.ShoppingCart.Where(
                    m =>     
                        m.ApplicationUserId.ToString().Equals(ShoppingCart.ApplicationUserId.ToString()) && 
                        m.MenuItemId.ToString().Equals(ShoppingCart.MenuItemId.ToString())
                ).FirstOrDefaultAsync();

                if (shoppingCartFromDb == null)
                {
                    await _db.ShoppingCart.AddAsync(ShoppingCart);
                }
                else
                {
                    shoppingCartFromDb.Count += ShoppingCart.Count;
                }
                await _db.SaveChangesAsync();

                var itemCount = (await _db.ShoppingCart.Where(c => c.ApplicationUserId.ToString().Equals(ShoppingCart.ApplicationUserId.ToString())).ToListAsync()).Count();
                HttpContext.Session.SetInt32("ssCount",itemCount);

                return RedirectToAction(nameof(Index));
            }

            var menuItemFromDb = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).FirstOrDefaultAsync(m => m.Id == ShoppingCart.MenuItemId);
            //if (menuItemFromDb == null) return NotFound();

            ShoppingCart shoppingCart = new ShoppingCart()
            {
                MenuItem = menuItemFromDb,
                MenuItemId = menuItemFromDb.Id,
            };

            return View(ShoppingCart);

        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

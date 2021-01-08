using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Spice.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.ManagerUser)]
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        [BindProperty]
        public ApplicationUser ApplicationUser { get; set; }
        public UserController(ApplicationDbContext db)
        {
            _db = db;
            ApplicationUser = new ApplicationUser();
        }

        public async Task<IActionResult> Index()
        {
            //get logged user
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            //if null user is not logged in; if not null, users id will be in claim variable
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            return View(await _db.ApplicationUser.Where(u=>u.Id!=claim.Value).ToListAsync());
        }

        
        public async Task<IActionResult> Lock(string id) 
        {
            if (id == null) return NotFound();
            var appUser = await _db.ApplicationUser.FirstOrDefaultAsync(m => m.Id == id);
            if (appUser == null) return NotFound();
            //it will be lock fo 1000 from now
            appUser.LockoutEnd = DateTime.Now.AddYears(1000);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UnLock(string id) 
        {
            if (id == null) return NotFound();
            var appUser = await _db.ApplicationUser.FirstOrDefaultAsync(m => m.Id == id);
            if (appUser == null) return NotFound();
            //it will be lock fo 1000 from now
            appUser.LockoutEnd = DateTime.Now;
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


    }
}

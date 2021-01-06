using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Spice.Areas.Admin.Controllers
{
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
    }
}

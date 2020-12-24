using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Spice.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Spice.Models.ViewModels;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MenuItemController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment; // this is in config method in startup.cs as parameter

        [BindProperty] //from .net core 3.1 we can use bind property instead of passing MenuItemViewModel as argument to every action method
        public MenuItemViewModel MenuItemViewModel { get; set; }

        public MenuItemController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            MenuItemViewModel = new MenuItemViewModel
            {
                MenuItem = new Models.MenuItem(),
                CategoryList = _db.Category,
                //SubCategoryList = await _db.SubCategory.ToListAsync()
            };
        }

        public async Task<IActionResult> Index()
        {
            var result = await _db.MenuItem.Include(s=> s.Category).Include(s=>s.SubCategory).ToListAsync();
            return View(result);
        }

        //GET - Create
        public async Task<IActionResult> Create()
        {
            return View(MenuItemViewModel);
        }
    }
}

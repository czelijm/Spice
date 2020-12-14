using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public SubCategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            //include for loading the properties connected by foregin key
            return View(await _db.SubCategory.Include(s=>s.Category).ToListAsync());
        }
        public async Task<IActionResult> Create()
        {
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = new Models.SubCategory(),
                SubCatogoryNameList = await _db.Category.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync()
            }; 

            return View(model);
        }

        [HttpPost]
        //[ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategory subCategory)
        {
            if (subCategory == null || !ModelState.IsValid)
            {
                return View(subCategory);
            }

            await _db.SubCategory.AddAsync(subCategory);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Extensions;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Spice.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.ManagerUser)]
    [Area("Admin")]
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        [TempData]
        public string StatusMessage { get; set; }
        public SubCategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            //include for loading the properties connected by foregin key
            return View(await _db.SubCategory.Include(s=>s.Category).ToListAsync());
        }
        //GET - Create
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

        //POST - Create
        [HttpPost]
        //[ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doesSubCategoryExist = _db.SubCategory.Include(s => s.Category).Where(s => s.Name == model.SubCategory.Name && s.Category.Id == model.SubCategory.CategoryId);
                if (doesSubCategoryExist.Count() > 0)
                {
                    StatusMessage = "Error : SubCategory exist under " + doesSubCategoryExist.First().Category.Name + " category. Please use another name.";
                    //Error
                    //return View(model);
                }
                else
                {
                    await _db.SubCategory.AddAsync(model.SubCategory);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            SubCategoryAndCategoryViewModel subCategoryAndCategoryViewModel = new SubCategoryAndCategoryViewModel
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCatogoryNameList = await _db.SubCategory.OrderBy(s => s.Name).Select(s=>s.Name).ToListAsync(),
                StatusMessage = StatusMessage
                
            };
            return View(subCategoryAndCategoryViewModel);
        }

        [ActionName("GetSubCategory")]
        public async Task<IActionResult> GetSubCategory(Guid id)
        {
            //List<SubCategory> subCategories = await _db.SubCategory.Where(s => s.CategoryId == id).ToListAsync();
            //var result = subCategories.ToSelectListItem(id);

            List<SubCategory> subCategories = new List<SubCategory>();
            subCategories = await (from subCategory in _db.SubCategory
                             where subCategory.CategoryId == id
                             select subCategory).ToListAsync();
            
            return Json(new SelectList(subCategories,"Id","Name"));
        }

        //GET - Edit
        //public async Task<IActionResult> Edit(Guid id) 
        //{
        //    if (id == null) { return NotFound(); }

        //    SubCategory result = await _db.SubCategory.Include(s => s.Category).FirstOrDefaultAsync(s => s.Id == id);

        //    if (result == null) { return NotFound();  }

        //    return View(result);

        //}

        //GET - Edit
        public async Task<IActionResult> Edit(Guid id)
        {
            if (id == null) { return NotFound(); }
            
            SubCategory result = await _db.SubCategory.SingleOrDefaultAsync(s=>s.Id==id);
            
            if (result == null) { return NotFound(); }

            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = result,
                SubCatogoryNameList = await _db.Category.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync()
            };

            return View(model);
        }

        //POST - Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(Guid id, SubCategoryAndCategoryViewModel model)
        public async Task<IActionResult> Edit(SubCategoryAndCategoryViewModel model) //if we use hidden input in the view we dont have to add additional argument here
        {
            if (ModelState.IsValid)
            {
                var doesSubCategoryExist = _db.SubCategory.Include(s => s.Category).Where(s => s.Name == model.SubCategory.Name && s.Category.Id == model.SubCategory.CategoryId);
                if (doesSubCategoryExist.Count() > 0)
                {
                    StatusMessage = "Error : SubCategory exist under " + doesSubCategoryExist.First().Category.Name + " category. Please use another name.";
                    //Error
                    //return View(model);
                }
                else
                {
                    //var tmpSubCat = await doesSubCategoryExist.FirstOrDefaultAsync(s=>s.Id == id);

                    var subCategoryFromDb = await _db.SubCategory.FindAsync(model.SubCategory.Id);
                    subCategoryFromDb.Name = model.SubCategory.Name; //Updating only one property
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            //model.SubCategory.Id = id;
            SubCategoryAndCategoryViewModel subCategoryAndCategoryViewModel = new SubCategoryAndCategoryViewModel
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCatogoryNameList = await _db.SubCategory.OrderBy(s => s.Name).Select(s => s.Name).ToListAsync(),
                StatusMessage = StatusMessage
            };
            return View(subCategoryAndCategoryViewModel);
        }

        //GET - Delete
        public async Task<IActionResult> Delete(Guid id) 
        {
            if (id == null) { return NotFound(); }
            //var result = await _db.SubCategory.Include(s => s.Category).Where(s => s.Id == id).FirstAsync();
            var result = await _db.SubCategory.Include(s => s.Category).SingleOrDefaultAsync(s => s.Id == id);
            if (result == null) { return NotFound(); }

            //SubCategoryAndCategoryViewModel subCategoryAndCategoryViewModel = new SubCategoryAndCategoryViewModel
            //{
            //    CategoryList = await _db.Category.ToListAsync(),
            //    SubCategory = result, //model.SubCategory,
            //    SubCatogoryNameList = null, //= await _db.SubCategory.OrderBy(s => s.Name).Select(s => s.Name).ToListAsync(),
            //    StatusMessage = null
            //};

            return View(result);
        }

        //POST - Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteOnPost(Guid id) 
        {
            if (id == null) return NotFound();

            var result = await _db.SubCategory.FindAsync(id);

            if (result == null) return NotFound();

            _db.SubCategory.Remove(result);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //GET - Details
        public async Task<IActionResult> Details(Guid id)
        {
            if (id == null) return RedirectToAction(nameof(Index));

            //var result = await _db.SubCategory.FindAsync(id);
            var result = await _db.SubCategory.Include(s => s.Category).SingleOrDefaultAsync(s => s.Id == id);

            if (result == null) return RedirectToAction(nameof(Index));

            //SubCategoryAndCategoryViewModel subCategoryAndCategoryViewModel = new SubCategoryAndCategoryViewModel
            //{
            //    CategoryList = await _db.Category.ToListAsync(),
            //    SubCategory = result, //model.SubCategory,
            //    SubCatogoryNameList = null, //= await _db.SubCategory.OrderBy(s => s.Name).Select(s => s.Name).ToListAsync(),
            //    StatusMessage = null
            //};

            return View(result);
        }

    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        //[BindProperty]
        //private Category Category { get; set; }

        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        //GET action method
        public async Task<IActionResult> Index()
        {
            return View(await _db.Category.ToListAsync());
        }

        //GET for Create
        public IActionResult Create()
        {
            return View();
        }

        //POST - Create
        [HttpPost]
        [ValidateAntiForgeryToken]//to prevent attacs
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                await _db.Category.AddAsync(category);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index)); // return to action method that call the view
            }

            return View(category);
        }

        //GET - Edit
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var result = await _db.Category.FindAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            return View(result);
        }

        //POST action method
        [HttpPost]
        [ValidateAntiForgeryToken]//to prevent attacks
        public async Task<IActionResult> Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _db.Category.Update(category);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        //Name of the argument must have this same name as the asp-route-Name
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var result = await _db.Category.FindAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            return View(result);
        }

        // [HttpPost,ActionName("Delete")] 
        [HttpPost, ActionName("Delete")] //for action method name DeleteConfirm, becouse there cannot exist action methods with this same name
                                         //It has to be the HttpPost not the HttpDelete
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> DeletePostAction(Guid id)
        {
            var result = await _db.Category.FindAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            _db.Category.Remove(result);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Details(Guid id) 
        {
            var result = await _db.Category.FindAsync(id);
            if (result==null)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(result);
        }

        //POST action method
        //[HttpPost]
        //[ValidateAntiForgeryToken]//to prevent attacks
        //public async Task<IActionResult> Upsert(Guid? id)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (id == null)
        //        {
        //            await _db.Category.AddAsync(Category);
        //            //await _db.SaveChangesAsync();
        //            //return RedirectToAction(nameof(Index));
        //        }
        //        else
        //        {
        //            _db.Category.UpdateRange(Category);
        //            //await _db.SaveChangesAsync();
        //            //return Redirect("Index");
        //        }
        //        await _db.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }

        //    return View(Category);
        //}
    }
}

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

        [BindProperty]
        private Category Category { get; set; }

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



        //POST action method
        public async Task<IActionResult> Upsert(Guid? guid)
        {
            if (ModelState.IsValid)
            {
                if (guid == null)
                {
                    await _db.Category.AddAsync(Category);

                }
                else
                {
                    _db.Category.UpdateRange(Category);
                    return Redirect("Index");
                }
            }

            return View();


        }

    }
}

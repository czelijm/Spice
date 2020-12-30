using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CouponController : Controller
    {
        private ApplicationDbContext _db { get; set; }
        
        [BindProperty]
        public Coupon Coupon { get; set; }

        public CouponController(ApplicationDbContext db)
        {
            _db = db;
            Coupon = new Coupon();
        }

        public async Task<IActionResult> Index()
        {
            return View(await _db.Coupon.ToListAsync());
        }

        //GET - create
        public async Task<IActionResult> Create() 
        {
            return View(Coupon);
        }

        //POST - Create
        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost()
        {
            if (!ModelState.IsValid)
            {
                return View(Coupon);
            }

            var files = HttpContext.Request.Form.Files;

            if (files.Count > 0)
            {
                //File Uploaded

                //image -> streamOfBytes
                byte[] p1 = null;
                using (var fs1 = files[0].OpenReadStream()) // start reading the file 
                {
                    using (var ms1 = new MemoryStream())//memory stream
                    {
                        await fs1.CopyToAsync(ms1);
                        p1 = ms1.ToArray();
                    }
                }
                Coupon.Picture = p1;
            }

            await _db.Coupon.AddAsync(Coupon);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //GET - edit
        public async Task<IActionResult> Edit(Guid id)
        {
            if (id == null) return NotFound();
            var result = await _db.Coupon.FindAsync(id);
            if (result == null) return NotFound();

            return View(result);
        }

        //POST - Create
        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(Guid id)
        {

            if (id == null) { return NotFound(); }

            if (!ModelState.IsValid)
            {
                return View(Coupon);
            }

            var modelFromDB = await _db.Coupon.FindAsync(id);

            if (modelFromDB == null) { return NotFound(); }

            var files = HttpContext.Request.Form.Files;

            if (files.Count > 0)
            {
                //File Uploaded

                //image -> streamOfBytes
                byte[] p1 = null;
                using (var fs1 = files[0].OpenReadStream()) // start reading the file 
                {
                    using (var ms1 = new MemoryStream())//memory stream
                    {
                        await fs1.CopyToAsync(ms1);
                        p1 = ms1.ToArray();
                    }
                }
                modelFromDB.Picture = p1;
            }

            modelFromDB.Name = Coupon.Name;
            modelFromDB.CouponType = Coupon.CouponType;
            modelFromDB.Dsicount = Coupon.Dsicount;
            modelFromDB.IsActive = Coupon.IsActive;
            modelFromDB.MinimumAmmount = Coupon.MinimumAmmount;
            
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //GET - delete
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == null) return NotFound();
            var result = await _db.Coupon.FindAsync(id);
            if (result == null) return NotFound();

            return View(result);
        }

        //POST - Create
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(Guid id)
        {

            if (id == null) { return NotFound(); }

            var modelFromDB = await _db.Coupon.FindAsync(id);

            if (modelFromDB == null) { return NotFound(); }

            _db.Coupon.Remove(modelFromDB);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //GET - delete
        public async Task<IActionResult> Details(Guid id)
        {
            if (id == null) return NotFound();
            var result = await _db.Coupon.FindAsync(id);
            if (result == null) return NotFound();

            return View(result);
        }

    }
}

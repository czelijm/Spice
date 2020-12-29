using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Spice.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Spice.Models.ViewModels;
using Spice.Utility;
using System.IO;

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Create")]
        public async Task<IActionResult> CreatePOST() 
        {
            //We have to add that, becouse we haven't assing value to SubCategoryId only populate list using javaScript (call Action method form controller) so whe have to add that using data from form.   
            MenuItemViewModel.MenuItem.SubCategoryId = new Guid(Request.Form["SubCategoryId"].ToString());

            if (!ModelState.IsValid)
            {
                return View(MenuItemViewModel);
            }

            await _db.MenuItem.AddAsync(MenuItemViewModel.MenuItem);
            await _db.SaveChangesAsync();

            //Image saving section--------------------------------------------------
            string webRootPath = _webHostEnvironment.WebRootPath;
            //Extracting Images that user has uploaded
            var files = HttpContext.Request.Form.Files;
            //Extract File from dataBase whatever have been saved
            var menuItemFromDb = await _db.MenuItem.FindAsync(MenuItemViewModel.MenuItem.Id);

            if (files.Count > 0)
            {
                //File uploaded
                var uploads = Path.Combine(webRootPath,"images");
                var extension = Path.GetExtension(files[0].FileName); // use only first uploaded file

                using (var filesStream = new FileStream(Path.Combine(uploads,MenuItemViewModel.MenuItem.Id.ToString()+extension),FileMode.Create))
                {
                    await files[0].CopyToAsync(filesStream);//copy stream to file to the server
                }
                //in db change image column to the location where iamge is saved
                menuItemFromDb.Image = @"\"+SD.ImageDefaulInnerPath + MenuItemViewModel.MenuItem.Id.ToString() + extension;
            }
            else 
            {
                //File Not uploaded, use default insted
                var uploads = Path.Combine(webRootPath,(SD.ImageDefaulInnerPath+SD.DefaultFoodImage));
                var imageInnerPath = @"\"+SD.ImageDefaulInnerPath + MenuItemViewModel.MenuItem.Id.ToString() + ".png";
                //System.IO.File.Copy(uploads, Path.Combine(webRootPath, imageInnerPath).ToString());
                System.IO.File.Copy(uploads, webRootPath + imageInnerPath);
                menuItemFromDb.Image = imageInnerPath;
            }
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //GET - EDIT
        public async Task<IActionResult> Edit(Guid id) 
        {
            if (id == null) { return NotFound(); }

            MenuItemViewModel.MenuItem = await _db.MenuItem.Include(s => s.Category).Include(s => s.SubCategory).SingleOrDefaultAsync(s => s.Id == id);
            MenuItemViewModel.SubCategoryList = await _db.SubCategory.Where(s => s.CategoryId == MenuItemViewModel.MenuItem.CategoryId).ToListAsync();

            if (MenuItemViewModel.MenuItem == null) { return NotFound(); }

            return View(MenuItemViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Edit")]
        public async Task<IActionResult> EditPOST(Guid id)
        {
            if (id == null) { return NotFound(); }

            //Becouse we assign it in form via JavaScript
             
            MenuItemViewModel.MenuItem.SubCategoryId = new Guid(Request.Form["SubCategoryId"].ToString());

            //MenuItemViewModel.MenuItem
            if (!ModelState.IsValid)
            {
                MenuItemViewModel.SubCategoryList = await _db.SubCategory.Where(s => s.CategoryId == MenuItemViewModel.MenuItem.SubCategoryId).ToListAsync();
                return View(MenuItemViewModel);
            }

            var menuItemFromDb = await _db.MenuItem.FindAsync(id);
            if (menuItemFromDb==null) { return NotFound(); }

            //await _db.SaveChangesAsync();

            //Image saving section--------------------------------------------------
            var files = HttpContext.Request.Form.Files;

            if (files.Count > 0)
            {
                string webRootPath = _webHostEnvironment.WebRootPath;
                //Extracting Images that user has uploaded

                var uploads = Path.Combine(webRootPath, "images");
                var extension = Path.GetExtension(files[0].FileName); // use only first uploaded file
                var imagePath = Path.Combine(webRootPath, menuItemFromDb.Image.TrimStart('\\'));

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                using (var filesStream = new FileStream(Path.Combine(uploads, MenuItemViewModel.MenuItem.Id.ToString() + extension), FileMode.Create))
                {
                    await files[0].CopyToAsync(filesStream);//copy stream to file to the server
                }
                menuItemFromDb.Image = @"\" + SD.ImageDefaulInnerPath + MenuItemViewModel.MenuItem.Id.ToString() + extension;
            }

            menuItemFromDb.Name = MenuItemViewModel.MenuItem.Name; 
            menuItemFromDb.Price = MenuItemViewModel.MenuItem.Price;
            menuItemFromDb.Spicyness = MenuItemViewModel.MenuItem.Spicyness;
            menuItemFromDb.SubCategoryId = MenuItemViewModel.MenuItem.SubCategoryId;
            menuItemFromDb.CategoryId= MenuItemViewModel.MenuItem.CategoryId;
            menuItemFromDb.Description= MenuItemViewModel.MenuItem.Description;


            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //GET - details
        public async Task<IActionResult> Details(Guid id)
        {
            if (id == null) { return NotFound(); }
            MenuItemViewModel.MenuItem = await _db.MenuItem.Include(s => s.Category).Include(s => s.SubCategory).SingleOrDefaultAsync(s => s.Id == id);
            if (MenuItemViewModel.MenuItem == null) { return NotFound(); }
            MenuItemViewModel.SubCategoryList = await _db.SubCategory.Where(s => s.CategoryId == MenuItemViewModel.MenuItem.SubCategoryId).ToListAsync();
            return View(MenuItemViewModel);
        }


        //GET - Delete
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == null) { return NotFound(); }
            MenuItemViewModel.MenuItem = await _db.MenuItem.Include(s => s.Category).Include(s => s.SubCategory).SingleOrDefaultAsync(s => s.Id == id);
            if (MenuItemViewModel.MenuItem == null) { return NotFound(); }
            MenuItemViewModel.SubCategoryList = await _db.SubCategory.Where(s => s.CategoryId == MenuItemViewModel.MenuItem.SubCategoryId).ToListAsync();
            return View(MenuItemViewModel);
        }

        //POST - Delete
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            if (id == null) { return NotFound(); }
            var menuItemFromDb = await _db.MenuItem.FindAsync(id);
            if (menuItemFromDb == null) { return NotFound(); }
            string webRootPath = _webHostEnvironment.WebRootPath;

            if (!(menuItemFromDb.Image is null ))
            {
                var imagePath = Path.Combine(webRootPath, menuItemFromDb.Image.TrimStart('\\'));

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            
            _db.MenuItem.Remove(menuItemFromDb);
            await _db.SaveChangesAsync();

            return (RedirectToAction(nameof(Index)));
        }
    }
}

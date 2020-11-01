using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using spice.Data;
using spice.Models;
using spice.Models.ViewModel;
using Spice.Utility;

namespace spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ManagerUser)]

    public class MenuItemController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _HostEnvironment;
        
        // If we dont want to get data by parameter
        [BindProperty]
        public MenuItemCreateViewModel MenuItemVM { get; set; }

        public MenuItemController(ApplicationDbContext db, IWebHostEnvironment HostEnvironment)
        {
            _db = db;
            _HostEnvironment = HostEnvironment;
            
            MenuItemVM = new MenuItemCreateViewModel()
            {

                //CategoryLists = new SelectList(_db.Category, "Id", "Name"),
                CategoryLists = new SelectList(_db.Category, "Id", "Name"),
                SubCategoryLists = _db.SubCategory,
                MenuItem = new Models.MenuItem()
            };
        }

        // GET: Admin/MenuItem
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = await _db.MenuItem
                                       .Include(m => m.Category)
                                       .Include(m => m.SubCategory)
                                       .ToListAsync();
            
            return View(applicationDbContext);
        }

        // GET: Admin/MenuItem/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menuItem = await _db.MenuItem
                .Include(m => m.Category)
                .Include(m => m.SubCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (menuItem == null)
            {
                return NotFound();
            }

            return View(menuItem);
        }

        // GET: Admin/MenuItem/Create
        public IActionResult Create()
        {
            // Declare these variable in a constructor
            //MenuItemVM = new MenuItemCreateViewModel();
            //var CategoryLists = await _db.Category.ToListAsync();
            //MenuItemVM.CategoryLists = new SelectList(CategoryLists, "Id", "Name");

            // We can write also this way......
            //MenuItemVM.SubCategoryLists = new SelectList(_db.SubCategory, "Id", "Name");
            //MenuItemVM.MenuItem = new Models.MenuItem();

            return View(MenuItemVM);
        }

        // POST: Admin/MenuItem/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST()
        {
            //int i;
            // We write Subcategory Id by Javascript, so we dont get it with all form values, so we need to take it by below.... 
            //MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString().Trim());
            
            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            if (!ModelState.IsValid)
            {
                return View(MenuItemVM);
            }

            _db.MenuItem.Add(MenuItemVM.MenuItem);
            await _db.SaveChangesAsync();

            //Work on the image saving section

            string webRootPath = _HostEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            // get Id that I saved now, and I will upload image in this row
            var menuItemFromDb = await _db.MenuItem.FindAsync(MenuItemVM.MenuItem.Id);

            if (files.Count > 0)
            {
                //files has been uploaded
                var uploads = Path.Combine(webRootPath, "images");
                var extension = Path.GetExtension(files[0].FileName);

                using (var filesStream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(filesStream);
                }
                menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + extension;
            }
            else
            {
                //no file was uploaded, so use default
                var uploads = Path.Combine(webRootPath, @"images\" + SD.DefaultFoodImage);
                System.IO.File.Copy(uploads, webRootPath + @"\images\" + MenuItemVM.MenuItem.Id + ".png");
                menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + ".png";
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/MenuItem/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = await _db.MenuItem.Include(m => m.Category)
                                  .Include(m => m.SubCategory)
                                  .SingleOrDefaultAsync(m => m.Id == id);

            MenuItemVM.SubCategoryLists = await _db.SubCategory
                                          .Where(s => s.CategoryId == MenuItemVM.MenuItem.CategoryId)
                                          .ToListAsync();
           
            if (MenuItemVM.MenuItem == null)
            {
                return NotFound();
            }
            return View(MenuItemVM);
        }

        // POST: Admin/MenuItem/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPOST(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            if (!ModelState.IsValid)
            {
                MenuItemVM.SubCategoryLists = await _db.SubCategory.Where(s => s.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();
                return View(MenuItemVM);
            }

            //Work on the image saving section

            string webRootPath = _HostEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var menuItemFromDb = await _db.MenuItem.FindAsync(MenuItemVM.MenuItem.Id);

            if (files.Count > 0)
            {
                //New Image has been uploaded
                var uploads = Path.Combine(webRootPath, "images");
                var extension_new = Path.GetExtension(files[0].FileName);

                //Delete the original file
                var imagePath = Path.Combine(webRootPath, menuItemFromDb.Image.TrimStart('\\'));

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                //we will upload the new file
                using (var filesStream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extension_new), FileMode.Create))
                {
                    files[0].CopyTo(filesStream);
                }
                menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + extension_new;
            }

            menuItemFromDb.Name = MenuItemVM.MenuItem.Name;
            menuItemFromDb.Description = MenuItemVM.MenuItem.Description;
            menuItemFromDb.Price = MenuItemVM.MenuItem.Price;
            menuItemFromDb.Spicyness = MenuItemVM.MenuItem.Spicyness;
            menuItemFromDb.CategoryId = MenuItemVM.MenuItem.CategoryId;
            menuItemFromDb.SubCategoryId = MenuItemVM.MenuItem.SubCategoryId;

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/MenuItem/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menuItem = await _db.MenuItem
                .Include(m => m.Category)
                .Include(m => m.SubCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (menuItem == null)
            {
                return NotFound();
            }

            return View(menuItem);
        }

        // POST: Admin/MenuItem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var menuItem = await _db.MenuItem.FindAsync(id);
            _db.MenuItem.Remove(menuItem);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MenuItemExists(int id)
        {
            return _db.MenuItem.Any(e => e.Id == id);
        }
    }
}

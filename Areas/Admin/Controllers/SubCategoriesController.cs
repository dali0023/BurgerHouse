using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using spice.Data;
using spice.Models;
using spice.Models.ModelView;
using Spice.Utility;

namespace spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ManagerUser)]

    public class SubCategoriesController : Controller
    {
        private readonly ApplicationDbContext _db;
        

       // TempData is a session
        [TempData]
        public string StatusMessage { get; set; }
        public SubCategoriesController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: Admin/SubCategories
        public async Task<IActionResult> Index()
        {
            var SubCategoriesInfo = await _db.SubCategory.Include(s => s.Category).ToListAsync();
            return View(SubCategoriesInfo);
        }

        // GET: Admin/SubCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subCategory = await _db.SubCategory
                .Include(s => s.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subCategory == null)
            {
                return NotFound();
            }

            return View(subCategory);
        }

        // GET: Admin/SubCategories/Create
        public async Task<IActionResult> Create()
        {
            var MySubCategoryViewModel = new SubCategoryCreateViewModel();

            var CategoryLists = await _db.Category.ToListAsync();
            MySubCategoryViewModel.CategoryLists = new SelectList(CategoryLists, "Id", "Name");
            MySubCategoryViewModel.SubCategory = new Models.SubCategory();

            MySubCategoryViewModel.SubCategoryList = await _db.SubCategory
                              .OrderBy(p => p.Name)
                              .Select(p => p.Name)
                              .Distinct()
                              .ToListAsync();         
            return View(MySubCategoryViewModel);
        }

        // POST: Admin/SubCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(SubCategoryCreateViewModel SubCategoryModelView)
        {
            if (ModelState.IsValid)
            {
                var DoesSubCategoriesExists = _db.SubCategory
                                              .Include(s=>s.Category)
                                              .Where(s => s.Name == SubCategoryModelView.SubCategory.Name
                                               && s.Category.Id == SubCategoryModelView.SubCategory.CategoryId);
                if (DoesSubCategoriesExists.Count() > 0)
                {
                    // Error
                    StatusMessage = $"Error: {SubCategoryModelView.SubCategory.Name} " +
                                    $"Exists Under {DoesSubCategoriesExists.First().Category.Name}";
                }
                else
                {
                    _db.Add(SubCategoryModelView.SubCategory);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            // Again Retrive everything from Database like Index method....
            var MySubCategoryViewModel = new SubCategoryCreateViewModel();

            var CategoryLists = await _db.Category.ToListAsync();
            MySubCategoryViewModel.CategoryLists = new SelectList(CategoryLists, "Id", "Name");
            MySubCategoryViewModel.SubCategory = SubCategoryModelView.SubCategory;

            MySubCategoryViewModel.SubCategoryList = await _db.SubCategory
                                                     .OrderBy(p => p.Name)
                                                     .Select(p => p.Name)
                                                     .Distinct()
                                                     .ToListAsync();
            MySubCategoryViewModel.StatusMessage = StatusMessage;
            return View(MySubCategoryViewModel);
        }

        // Get SubCategory
        [ActionName("GetSubCategory")]
        public async Task<IActionResult> GetSubCategory(int id)
        {
            // Declare a List
            var subCategories = new List<SubCategory>();

            //Get Data by matching Id
            //subCategories = await _db.SubCategory
            //                .Where(s => s.CategoryId == id)
            //                .ToListAsync();

            subCategories = await (from subCategory in _db.SubCategory
                     where subCategory.CategoryId == id
                     select subCategory).ToListAsync();

            return Json(new SelectList(subCategories, "Id", "Name"));
        }

       
        // GET: Admin/SubCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var subCategory = await _db.SubCategory.FindAsync(id);
            var subCategory = await _db.SubCategory.SingleOrDefaultAsync(m => m.Id == id);
            if (subCategory == null)
            {
                return NotFound();
            }

            var MySubCategoryViewModel = new SubCategoryCreateViewModel();
            var CategoryLists = await _db.Category.ToListAsync();
            MySubCategoryViewModel.CategoryLists = new SelectList(CategoryLists, "Id", "Name");
            MySubCategoryViewModel.SubCategory = subCategory;

            MySubCategoryViewModel.SubCategoryList = await _db.SubCategory
                              .OrderBy(p => p.Name)
                              .Select(p => p.Name)
                              .Distinct()
                              .ToListAsync();
            
            return View(MySubCategoryViewModel);
        }

        // POST: Admin/SubCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SubCategoryCreateViewModel SubCategoryModelView)
        {
            if (ModelState.IsValid)
            {
                var DoesSubCategoriesExists = _db.SubCategory
                                              .Include(s => s.Category)
                                              .Where(s => s.Name == SubCategoryModelView.SubCategory.Name
                                               && s.Category.Id == SubCategoryModelView.SubCategory.CategoryId);
                if (DoesSubCategoriesExists.Count() > 0)
                {
                    // Error
                    StatusMessage = $"Error: {SubCategoryModelView.SubCategory.Name} " +
                                    $"Exists Under {DoesSubCategoriesExists.First().Category.Name}";
                }
                else
                {
                    var GetSubCategoryFrom_Db = await _db.SubCategory.FindAsync(id);
                    GetSubCategoryFrom_Db.Name = SubCategoryModelView.SubCategory.Name;
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            // Again Retrive everything from Database like Index method....
            var MySubCategoryViewModel = new SubCategoryCreateViewModel();

            var CategoryLists = await _db.Category.ToListAsync();
            MySubCategoryViewModel.CategoryLists = new SelectList(CategoryLists, "Id", "Name");
            MySubCategoryViewModel.SubCategory = SubCategoryModelView.SubCategory;

            MySubCategoryViewModel.SubCategoryList = await _db.SubCategory
                                                     .OrderBy(p => p.Name)
                                                     .Select(p => p.Name)
                                                     .Distinct()
                                                     .ToListAsync();
            MySubCategoryViewModel.StatusMessage = StatusMessage;
            return View(MySubCategoryViewModel);
        }


        // GET: Admin/SubCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subCategory = await _db.SubCategory
                .Include(s => s.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subCategory == null)
            {
                return NotFound();
            }

            return View(subCategory);
        }

        // POST: Admin/SubCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subCategory = await _db.SubCategory.FindAsync(id);
            _db.SubCategory.Remove(subCategory);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubCategoryExists(int id)
        {
            return _db.SubCategory.Any(e => e.Id == id);
        }
    }
}

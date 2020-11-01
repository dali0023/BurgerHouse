using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spice.Data;
using spice.Models;
using Spice.Utility;

namespace spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ManagerUser)]

    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _db.Category.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        // use [httpPost] in every Post methods
        [HttpPost]
        // create validation anti token
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            // check form data valid or not
            if (ModelState.IsValid)
            {
                _db.Category.Add(category);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();

        }
        // get Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            // same
            //var category = await _db.Category.FirstOrDefaultAsync(m => m.Id == id);

            var category = await _db.Category.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }


        // Post Edit
        [HttpPost]
        // create validation anti token
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            // check form data valid or not
            if (ModelState.IsValid)
            {
                _db.Category.Update(category);
                // same
                //_db.Update(category);

                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();

        }

        // Get Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var category = await _db.Category.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // Post Delete
        [HttpPost, ActionName("Delete")]
        [AutoValidateAntiforgeryToken]

        //delete functions are two, both structure are same, 
        //if use ActionName("Delete"), then can use same structore like (int? id)
        //if we want to use (int id), we dont need action Name
        public async Task<IActionResult> ConfirmDelete(int? id)
        {          
            var category = await _db.Category.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            _db.Category.Remove(category);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Get Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var category = await _db.Category.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
    }
}
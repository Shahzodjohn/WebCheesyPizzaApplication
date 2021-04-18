using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebCheesyPizzaApplication.Context;
using WebCheesyPizzaApplication.Models;
using WebCheesyPizzaApplication.ViewModels;

namespace WebCheesyPizzaApplication.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CreateCategory() => View();
        [HttpPost]
        public async Task<IActionResult> CreateCategory(CategoryViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            await _context.Categories.AddAsync(new Category { Name = model.Name });
            await _context.SaveChangesAsync();
            return RedirectToAction("indexAdmin", "Products");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int? id)
        {
            if (id != null)
            {
                Category category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                return RedirectToAction("IndexAdmin","Products");

            }
            return NotFound();
        }
        [HttpGet]
        public async Task<IActionResult> UpdateCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            var updateCategory = new UpdateCategoryViewModel
            {
                Name = category.Name,
                Id = category.Id
            };
            return View(updateCategory);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateCategory(UpdateCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            Category category = await _context.Categories.FindAsync(model.Id);
            category.Name = model.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction("IndexAdmin", "Products");


            //var updateCategory = await _context.Categories.FindAsync(new CategoryViewModel { Id = model.Id });
            //await _context.Categories.Update(updateCategory);
        } 
    }
}

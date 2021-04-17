using Microsoft.AspNetCore.Mvc;
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
    }
}

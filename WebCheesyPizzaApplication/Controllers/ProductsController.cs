using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebCheesyPizzaApplication.Context;
using WebCheesyPizzaApplication.Models;
using WebCheesyPizzaApplication.ViewModels;
using static System.Net.WebRequestMethods;

namespace WebCheesyPizzaApplication.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<IdentityUser> _userManager;

        public ProductsController(IWebHostEnvironment environment, AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _environment = environment;
            _context = context;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                return Redirect("/Products/IndexAdmin");
            }
            var categories = await _context.Categories.Select(x => new CategoryViewModel { Id = x.Id, Name = x.Name }).ToListAsync();
            var products = await _context.Products.Include(s =>
            s.Category).Select(s => new ProductViewModel { Id = s.Id, Name = s.Name, CategoryName = s.Category.Name, ImagePath = s.Image, Price = s.Price }).ToListAsync();

            return View(new IndexCategoryViewModel { Categories = categories, Products = products });
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> IndexAdmin()
        {
            var categories = await _context.Categories.Select(x => new CategoryViewModel { Id = x.Id, Name = x.Name }).ToListAsync();
            var products = await _context.Products.Include(s => s.Category).Select(s => new ProductViewModel { Id = s.Id, Name = s.Name, CategoryName = s.Category.Name, ImagePath = s.Image, Price = s.Price }).ToListAsync();
            return View(new IndexCategoryViewModel { Categories = categories, Products = products });
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProduct()
        {
            var categories = await _context.Categories.ToDictionaryAsync(x => x.Id, x => x.Name);
            return View(new CreateProductViewModel { Categories = categories });
        }
        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateProductViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            string directoryPath = CreateDirectory();
            await AddFile(model.Image, directoryPath);
            var product = new Product
            {
                Image = "/images/" + model.Image.FileName,
                CategoryId = model.CategoryId,
                Name = model.Name,
                Price = model.Price,
                Description = model.Description,
                Reciept = model.Reciept,
            };
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");

        }

        public async Task<IActionResult> DetailsProduct(int id)
        {
            var productViewModel = await _context.Products.Select(x =>
            new ProductViewModel { Id = x.Id, CategoryName = x.Category.Name, Name = x.Name, ImagePath = x.Image, Price = x.Price, Description = x.Description, Reciept = x.Reciept }).FirstOrDefaultAsync(x => x.Id == id);
            return View(productViewModel);

        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int? id)
        {
            if (id != null)
            {
                Product product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return RedirectToAction("IndexAdmin");

            }
            return NotFound();
        }

        public async Task<IActionResult> Search(string searchString)
        {
            var products = from m in _context.Products
                           select m;
            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(s => s.Name.Contains(searchString));
            }
            return View(await products.ToListAsync());
        }


        private static async Task AddFile(IFormFile file, string directoryPath)
        {
            string imagePath = directoryPath + $"/{file.FileName}";
            using (FileStream fs = new FileStream(imagePath, FileMode.Create))
            {
                await file.CopyToAsync(fs);
            }
        }

        private string CreateDirectory()
        {
            string directoryPath = _environment.WebRootPath + "/Images";
            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);
            return directoryPath;
        }
        public async Task<IActionResult> SearchByCategories(int CategoryId)
        {
            var search = await _context.Products.Include(x => x.Category).Where(s => s.CategoryId == CategoryId).Select(x =>
              new ProductViewModel { Id = x.Id, Name = x.Name, ImagePath = x.Image, CategoryName = x.Category.Name, Price = x.Price }).ToListAsync();
            return View(search);
        }
        public async Task<IActionResult> List()
        {
            var categories = await _context.Categories.Select(x => new CategoryViewModel { Id = x.Id, Name = x.Name }).ToListAsync();
            return View(categories);
        }
        public IActionResult FreeBasket()
        {
            return View();
        }

        public async Task<IActionResult> BasketProducts()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var CurrentUserBasket = await _context.Baskets.FirstOrDefaultAsync(x => x.UserId == currentUser.Id);
            if (CurrentUserBasket == null)
                return RedirectToAction("FreeBasket");
            var currentUserCartProductNames = await (from bp in _context.BasketProducts
                                                     join p in _context.Products on bp.ProductId equals p.Id
                                                     where bp.BasketId == CurrentUserBasket.Id
                                                     select new BasketProductViewModel { Name = p.Name, Id = p.Id, Amount = bp.Amount, Price = p.Price }).ToListAsync();
            return View(currentUserCartProductNames);
        }
        [Authorize(Roles = ("User"))]
        public async Task<IActionResult> AddToBasket(int id)
        {

            var currentUser = await _userManager.GetUserAsync(User);
            var basket = await _context.Baskets.FirstOrDefaultAsync(x => x.UserId == currentUser.Id);
            if (basket == null)
            {
                basket = new Basket
                {
                    UserId = currentUser.Id
                };
                await _context.Baskets.AddAsync(basket);

                await _context.SaveChangesAsync();
            }
            var basketProduct = await _context.BasketProducts.FirstOrDefaultAsync(x => x.BasketId == basket.Id && x.ProductId == id);
            if (basketProduct == null)
            {
                basketProduct = new BasketProducts
                {
                    ProductId = id,
                    BasketId = basket.Id,
                    Amount = 1,
                };
                await _context.BasketProducts.AddAsync(basketProduct);
                await _context.SaveChangesAsync();
            }
            else
            {
                basketProduct.Amount++;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("BasketProducts");
        }
        public async Task<IActionResult> DeleteProductFromBasket(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var basket = await _context.Baskets.FirstOrDefaultAsync(x => x.UserId == currentUser.Id);
            var basketProduct = await _context.BasketProducts.FirstOrDefaultAsync(x => x.BasketId == basket.Id && x.ProductId == id);
            if (basketProduct.Amount == 1)
            {
                _context.BasketProducts.Remove(basketProduct);
            }
            else
            {
                basketProduct.Amount--;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("BasketProducts");
        }
        [Authorize]
        public async Task<IActionResult> Orderings()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var basket = await _context.Baskets.FirstOrDefaultAsync(x => x.UserId == currentUser.Id);
            var basketProducts = await _context.BasketProducts.Where(x => x.BasketId == basket.Id).ToListAsync();

            var order = new Order
            {
                OrderDate = DateTime.Now,
                OrderStateId = 2,
                UserId = currentUser.Id
            };

        }








    }
}

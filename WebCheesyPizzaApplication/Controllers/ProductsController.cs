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
        [HttpGet]
        public async Task<IActionResult> UpdateProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            var updateProduct = new UpdateViewModel
            {
                Categories = await _context.Categories.ToDictionaryAsync(x => x.Id, x => x.Name),
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                Reciept = product.Reciept
                
            };
            return View(updateProduct);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProduct(UpdateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await _context.Categories.ToDictionaryAsync(x => x.Id, x => x.Name);

                return View(model);
            }
            var product = await _context.Products.FindAsync(model.Id);
            product.Name = model.Name;
            product.Price = model.Price;
            product.Description = model.Description;
            product.Reciept = model.Reciept;
            string directoryPath = CreateDirectory();
            string prevImagePath = _environment.WebRootPath + $"{product.Name}";
            System.IO.File.Delete(prevImagePath);
            product.Image = model.Image != null ? "images" + model.Image.FileName : product.Image;

            await _context.SaveChangesAsync();

            return RedirectToAction("index");

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
        public async Task<IActionResult> DeleteTheWholeProduct(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var basket = await _context.Baskets.FirstOrDefaultAsync(x => x.UserId == currentUser.Id);
            var basketProduct = await _context.BasketProducts.FirstOrDefaultAsync(x => x.BasketId == basket.Id && x.ProductId == id);
                _context.BasketProducts.Remove(basketProduct);
            
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
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            var orderProducts = basketProducts.Select(x => new OrderProduct { OrderId = order.Id, ProductId = x.ProductId, Amount = x.Amount }).ToList();
            await _context.OrderProducts.AddRangeAsync(orderProducts);
            _context.BasketProducts.RemoveRange(basketProducts);
            await _context.SaveChangesAsync();
            return RedirectToAction("Orders");
        }
       
        [Authorize]
        public async Task<IActionResult> OrderProducts(int orderId)
        {
            var orderProducts = await _context.OrderProducts.Where(x => x.OrderId == orderId).Include(x => x.Product).Select(x => new OrderProductViewModel { Name = x.Product.Name, Amount = x.Amount, Price = x.Product.Price }).ToListAsync();
            return View(orderProducts);
        }
        [Authorize]
        public async Task<IActionResult> Orders()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var orders = await _context.Orders.Where(x => x.UserId == currentUser.Id).Include(x => x.OrderProducts).ThenInclude(x => x.Product).Include(x => x.OrderState).ToListAsync();
            var orderViewModel = orders.Select(x => new OrderViewModel
            {
                Id = x.Id,
                OrderDate = x.OrderDate,
                State = x.OrderState.Name,
                Summ = x.OrderProducts.Sum(x => x.Amount*x.Product.Price)
            }).ToList();
            return View(orderViewModel);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> OrderList(int id)
        {
            var orderListViewModel = new OrderListViewModel
            {
                Orders = await _context.Orders.Include(x => x.OrderState).Include(x=>x.User).Select(x =>
                new OrderViewModel { Id = x.Id, OrderDate = x.OrderDate, State = x.OrderState.Name, UserId = x.UserId, UserName = x.User.UserName }
                ).ToListAsync(),
                OrderStates = await _context.OrderStates.ToDictionaryAsync(x => x.Id, x => x.Name)
            };

            return View(orderListViewModel);
        }


        [Authorize]
        public async Task<IActionResult> ChangeState(int newStateId, int orderId, string userId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == orderId && x.UserId == userId);
            order.OrderStateId = newStateId;
            await _context.SaveChangesAsync();
            return RedirectToAction("OrderList");
        }







    }
}

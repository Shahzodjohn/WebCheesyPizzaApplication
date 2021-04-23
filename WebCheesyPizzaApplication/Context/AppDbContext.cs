using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebCheesyPizzaApplication.Models;

namespace WebCheesyPizzaApplication.Context
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }


        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<BasketProducts> BasketProducts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderState> OrderStates { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }




        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<IdentityUser>().ToTable("Users");
            builder.Entity<OrderProduct>().HasKey(x => new { x.OrderId, x.ProductId });
            builder.Entity<Category>().HasData(new Category { Id = 1, Name = "Бургеры" });
            builder.Entity<OrderState>().HasData(
                new OrderState { Id = 1, Name = "Approved"},
                new OrderState { Id = 2, Name = "New"},
                new OrderState { Id = 3, Name = "Rejected"}
                );
            builder.Entity<Product>().HasData(new Product { Id = 1, CategoryId = 1, Image = "/images/burger.jpg", Name = "Гамбургер", Price = 12 });
        }
    }
}

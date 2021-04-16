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


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<IdentityUser>().ToTable("Users");

            builder.Entity<Category>().HasData(new Category { Id = 1, Name = "Бургеры" });
            builder.Entity<Product>().HasData(new Product { Id = 1, CategoryId = 1, Image = "/images/burger.jpg", Name = "Гамбургер", Price = 12 });
        }
    }
}

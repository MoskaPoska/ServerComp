using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProduct
{
    public class ShopContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public ShopContext(DbContextOptions<ShopContext> options) : base(options)
        {
            Database.EnsureCreated();
        }      
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=ProductDb;Trusted_Connection=True;");
        //    }
        //}
        //2 drop db и вставка
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasData(new Product[]
            {
                new Product{ID = 1, Category = "Ноутбук", Title = "Асус 3тысячи", Price = 100},
                new Product{ID = 2, Category = "Телефон", Title = "Редми Нот 8про", Price = 60},
                new Product{ID = 3, Category = "Планшет", Title = "Самсунг 3 таб", Price = 55},
                new Product{ID = 4, Category = "Компьютер", Title = "ХайперХ", Price = 133},
            });
        }
    }
}



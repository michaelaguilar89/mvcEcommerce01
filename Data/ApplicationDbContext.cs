using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MVCEcommerce.Models;

namespace MVCEcommerce.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categorys { get; set; }
        public DbSet<Product> Products { get;set; }
        public DbSet<image> Images { get; set; }
       
    }
}
